﻿using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore
{
    /// <summary>
    /// Class that manages actions from players
    /// </summary>
    public class Action
    {
        private readonly IGame _game;
        public Action(IGame game)
        {
            _game = game;
        }

        public event Delegates.StringString AfterAction;
        public event Delegates.String MultiplayerAction;

        //Make serialized action
        public void Make(string actionType, string[] args)
        {
            switch (actionType)
            {
                case Types.PlaceCharacter:
                {
                    Character character = _game.Characters.FirstOfID(int.Parse(args[0]));
                    HexCell cell = _game.HexMap.Cells.First(c => c.Coordinates.ToString() == args[1]);
                    PlaceCharacter(character, cell, true);
                } break;
                case Types.FinishTurn:
                {
                    FinishTurn(true);
                } break;
                case Types.TakeAction:
                {
                    Character character = _game.Characters.FirstOfID(int.Parse(args[0]));
                    TakeTurn(character, true);
                } break;
                case Types.BasicMove:
                {
                    Character characterToMove = _game.Characters.FirstOfID(int.Parse(args[0]));
                    List<HexCell> cellsToMove = args.Skip(1)
                        .Select(coords => _game.HexMap.Cells.First(c => c.Coordinates.ToString() == coords)).ToList();
                    BasicMove(characterToMove, cellsToMove, true);
                } break;
                case Types.BasicAttack:
                {
                    Character c1 = _game.Characters.FirstOfID(int.Parse(args[0]));
                    Character c2 = _game.Characters.FirstOfID(int.Parse(args[1]));
                    BasicAttack(c1, c2, true);
                } break;
                case Types.ClickAbility:
                {
                    var ability = _game.Abilities.FirstOfID(int.Parse(args[0])) as IClickable;
                    ClickAbility(ability, true);
                } break;
                case Types.UseAbility:
                {
                    Ability ability = _game.Abilities.FirstOfID(int.Parse(args[0]));
                    if (!args[1].EndsWith(")"))
                    {
                        Character character = _game.Characters.FirstOfID(int.Parse(args[1]));
                        UseAbility(ability as IUseableCharacter, character, true);
                        return;
                    }
                    List<HexCell> targetCells = args.Skip(1)
                        .Select(c => _game.HexMap.Cells.First(a => a.Coordinates.ToString() == c)).ToList();
                    if(targetCells.Count == 1)
                        UseAbility(ability as IUseableCell, targetCells.First(), true);
                    else
                        UseAbility(ability as IUseableCellList, targetCells, true);
                } break;
                case Types.Cancel:
                {
                    Cancel(true);
                } break;
                case Types.Select:
                {
                    Character character = _game.Characters.FirstOfID(int.Parse(args[0]));
                    Select(character, true);
                } break;

                case Types.Deselect:
                {
                    Deselect(true);
                } break;
                case Types.ExecuteCommand:
                {
                    ExecuteCommand(args[0], true);
                } break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }
        }

        public void PlaceCharacter(Character character, HexCell targetCell, bool force = false)
            => Act(Types.PlaceCharacter, $"{character.ID}:{targetCell.Coordinates.ToString()}",
                () => _game.HexMap.Place(character, targetCell), force);

        public void FinishTurn(bool force = false)
            => Act(Types.FinishTurn, () => _game.Active.Turn.Finish(), force);

        public void TakeTurn(Character character, bool force = false)
            => Act(Types.TakeAction, character.ID.ToString(), character.TryToTakeTurn, force);

        public void BasicMove(Character character, List<HexCell> cellPath, bool force = false)
            => Act(Types.BasicMove,
                $"{character.ID}:{string.Join(":", cellPath.Select(c => c.Coordinates.ToString()))}",
                () =>
                {
                    character.TryToTakeTurn();
                    character.BasicMove(cellPath);
                }, force);

        public void BasicAttack(Character character, Character target, bool force = false)
            => Act(Types.BasicAttack, $"{character.ID}:{target.ID}", () =>
            {
                character.TryToTakeTurn();
                character.BasicAttack(target);
            }, force);

        public void ClickAbility(IClickable ability, bool force = false)
            => Act(Types.ClickAbility, ((NKMEntity) ability).ID.ToString(), ability.Click, force);

        public void UseAbility(IUseableCell ability, HexCell cell, bool force = false)
            => Act(Types.UseAbility, $"{((NKMEntity) ability).ID.ToString()}:{cell.Coordinates.ToString()}", () => ability.Use(cell), force);

        public void UseAbility(IUseableCellList ability, IEnumerable<HexCell> cells, bool force = false)
        {
            IEnumerable<HexCell> hexCells = cells.ToList();
            Act(Types.UseAbility,
                $"{((NKMEntity) ability).ID.ToString()}:{string.Join(":", hexCells.Select(c => c.Coordinates.ToString()))}",
                () => ability.Use(hexCells.ToList()), force);
        }

        public void UseAbility(IUseableCharacter ability, Character character, bool force = false)
            => Act(Types.UseAbility, $"{((NKMEntity) ability).ID.ToString()}:{character.ID}", () => ability.Use(character), force);

        public void Cancel(bool force = false)
            => Act(Types.Cancel, () => _game.Active.Cancel(), force);

        public void Select(Character character, bool force = false)
            => Act(Types.Select, character.ID.ToString(), () => _game.Active.Select(character), force);

        public void Deselect(bool force = false)
            => Act(Types.Deselect, () => _game.Active.Deselect(), force);

        public void ExecuteCommand(string command, bool force = false)
            => Act(Types.ExecuteCommand, command, () => _game.Console.ExecuteCommand(command), force);

        private void Act(string actionType, System.Action action, bool force)
            => Act(actionType, "", action, force);
        private void Act(string actionType, string serializedContent, System.Action action, bool force)
        {
            string serializedMessage = Serialize(actionType, serializedContent);
            if (_game.Dependencies.Type == GameType.Multiplayer && !force)
                MultiplayerAction?.Invoke(serializedMessage);
            else
            {
                action();
                AfterAction?.Invoke(actionType, serializedContent);
            }
        }

        public static string Serialize(string actionType, string serializedContent)
            => string.Join(";", $"ACTION {actionType}", serializedContent);

        public static class Types
        {
            public const string PlaceCharacter = "PlaceCharacter";
            public const string FinishTurn = "FinishTurn";
            public const string TakeAction = "TakeAction";
            public const string BasicMove = "BasicMove";
            public const string BasicAttack = "BasicAttack";
            public const string ClickAbility = "ClickAbility";
            public const string UseAbility = "UseAbility";
            public const string Cancel = "Cancel";
            public const string Select = "Select";
            public const string Deselect = "Deselect";
            public const string OpenSelectable = "OpenSelectable";
            public const string CloseSelectable = "CloseSelectable";
            public const string ExecuteCommand = "ExecuteCommand";
        }
    }
}