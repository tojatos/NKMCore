using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore
{
    /// <summary>
    /// Class that manages actions from players
    /// </summary>
    public class Action
    {
        private readonly Game _game;
        public Action(Game game)
        {
            _game = game;
        }

        public event Delegates.String AfterAction;
        public event Delegates.String MultiplayerAction;
        
        //Make action from server
        public void Make(string actionType, string[] args)
        {
            switch (actionType)
            {
                case Types.PlaceCharacter:
                    Character character = _game.Active.GamePlayer.Characters.First(c => c.Name == args[0]);
                    HexCell cell = _game.HexMap.Cells.First(c => c.Coordinates.ToString() == args[1]);
                    PlaceCharacter(character, cell, true);
                    break;
                case Types.FinishTurn:
                    FinishTurn(true);
                    break;
                case Types.TakeAction:
                    Character character1 = _game.Active.GamePlayer.Characters.First(c => c.Name == args[0]);
                    TakeTurn(character1, true);
                    break;
                case Types.BasicMove:
                    Character characterToMove = _game.Active.GamePlayer.Characters.First(c => c.Name == args[0]);
                    List<HexCell> cellsToMove = args.Skip(1)
                        .Select(coords => _game.HexMap.Cells.First(c => c.Coordinates.ToString() == coords)).ToList();
                    BasicMove(characterToMove, cellsToMove, true);
                    break;
                case Types.BasicAttack:
                    Character c1 = _game.Active.GamePlayer.Characters.First(c => c.Name == args[0]);
                    Character c2 = _game.Characters.First(c => c.Name == args[1] && c1.CanBasicAttack(c));
                    BasicAttack(c1, c2, true);
                    break;
                case Types.ClickAbility:
                    IClickable ability = _game.Active.Character.Abilities.First(a => a.GetType().Name == args[0]) as IClickable;
                    ClickAbility(ability, true);
                    break;
                case Types.UseAbility:
                    Ability ab = _game.Active.Character.Abilities.First(a => a.GetType().Name == args[0]);
                    Character c3 = _game.Characters.First(c => c.Name == args[1]);
                    if (!args[1].EndsWith(")")) 
                        UseAbility(ab as IUseableCharacter, c3, true);
                    IEnumerable<HexCell> targetCells = args.Skip(1)
                        .Select(c => _game.HexMap.Cells.First(a => a.Coordinates.ToString() == c));
                    if(targetCells.Count() == 1)
                        UseAbility(ab as IUseableCell, targetCells[0]);
                    else 
                        UseAbility(ab as IUseableCellList, targetCells);
                    break;
                case Types.CancelAbility:
                    Cancel(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
            }
        }

        public void PlaceCharacter(Character character, HexCell targetCell, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.PlaceCharacter};{character.Name};{targetCell.Coordinates.ToString()}");
                return;
            }
            _game.HexMap.Place(character, targetCell);
            AfterAction?.Invoke(Types.PlaceCharacter);
        }

        public void FinishTurn(bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.FinishTurn}");
                return;
            }
            _game.Active.Turn.Finish();
            AfterAction?.Invoke(Types.FinishTurn);
        }

        public void TakeTurn(Character character, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.TakeAction};{character.Name}");
                return;
            }
            character.TryToTakeTurn();
            AfterAction?.Invoke(Types.TakeAction);
        }

        public void BasicMove(Character character, List<HexCell> cellPath, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.BasicMove};{character.Name}:{string.Join(":", cellPath.Select(c => c.Coordinates.ToString()))}");
                return;
            }
            character.TryToTakeTurn();
            character.BasicMove(cellPath);
            AfterAction?.Invoke(Types.BasicMove);
        }

        public void BasicAttack(Character character, Character target, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.BasicAttack};{character.Name}:{target.Name}");
                return;
            }
            character.TryToTakeTurn();
            character.BasicAttack(target);
            AfterAction?.Invoke(Types.BasicAttack);
        }

        public void ClickAbility(IClickable ability, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.ClickAbility};{ability.GetType().Name}");
                return;
            }
            ability.Click();
            AfterAction?.Invoke(Types.ClickAbility);
        }

        public void UseAbility(IUseableCell ability, HexCell cell, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.UseAbility};{ability.GetType().Name}:{cell.Coordinates.ToString()}");
                return;
            }
            ability.Use(cell);
            AfterAction?.Invoke(Types.UseAbility);
        }
        public void UseAbility(IUseableCellList ability, IEnumerable<HexCell> cells, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.UseAbility};{ability.GetType().Name}:{string.Join(":", cells.Select(c => c.Coordinates.ToString()))}");
                return;
            }
            ability.Use(cells.ToList());
            AfterAction?.Invoke(Types.UseAbility);
        }

        public void UseAbility(IUseableCharacter ability, Character character, bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.UseAbility};{ability.GetType().Name}:{character.Name}");
                return;
            }
            ability.Use(character);
            AfterAction?.Invoke(Types.UseAbility);
        }

        public void Cancel(bool force = false)
        {
            if (_game.Options.Type == GameType.Multiplayer && !force)
            {
                MultiplayerAction?.Invoke($"ACTION {Types.CancelAbility}");
                return;
            }
            _game.Active.Cancel();
            AfterAction?.Invoke(Types.CancelAbility);
        }

        public void Select(Character character)
        {
            _game.Active.Select(character);
            AfterAction?.Invoke(Types.Select);
        }

        public void Deselect()
        {
            _game.Active.Deselect();
            AfterAction?.Invoke(Types.Deselect);
        }
        
    
        public static class Types
        {
            public const string PlaceCharacter = "PlaceCharacter";
            public const string FinishTurn = "FinishTurn";
            public const string TakeAction = "TakeAction";
            public const string BasicMove = "BasicMove";
            public const string BasicAttack = "BasicAttack";
            public const string ClickAbility = "ClickAbility";
            public const string UseAbility = "UseAbility";
            public const string CancelAbility = "CancelAbility";
            public const string Select = "Select";
            public const string Deselect = "Deselect";
            public const string TouchCell = "TouchCell";
        }
    }
}