using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    /// <summary>
    /// This class is responsible for assigning characters to players
    /// </summary>
    public abstract class CharacterPick
    {
        protected readonly Game _game;
        protected readonly GamePreparerDependencies _preparerDependencies;

        protected CharacterPick(Game game, GamePreparerDependencies preparerDependencies)
        {
            _game = game;
            _preparerDependencies = preparerDependencies;
        }

        public static CharacterPick Create(Game game, GamePreparerDependencies preparerDependencies)
        {
            if (preparerDependencies.GameType == GameType.Multiplayer)
                return new ServerPick(game, preparerDependencies);
            switch (preparerDependencies.PickType)
            {
                case PickType.Blind:
                    return new BlindPick(game, preparerDependencies);
                case PickType.Draft:
                    return new DraftPick(game, preparerDependencies);
                case PickType.AllRandom:
                    return new AllRandom(game, preparerDependencies);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual Task BindCharactersToPlayers()
        {
            throw new NotImplementedException();
        }

        protected async Task PickCharacters(List<Character> charactersToPick, int numberOfCharactersToPick, GamePlayer player, Game game)
        {
            var s = new SelectableProperties
            {
                WhatIsSelected = SelectableProperties.Type.Character,
                IdsToSelect = charactersToPick.Select(c => c.ID).ToList(),
                ConstraintOfSelection = list => list.Count == numberOfCharactersToPick,
                OnSelectFinish = list => player.Characters.AddRange(list.Select(id => CharacterFactory.Create(game, charactersToPick.Single(c => c.ID == id).Name))),
                SelectionTitle = $"Wybór postaci - {player.Name}",
            };
            await SelectAndWait(s);
        }

        protected async Task SelectAndWait(SelectableProperties props)
        {
            bool isSelected = false;
            props.OnSelectFinish += list => isSelected = true;
            int selectableIndex = _game.Dependencies.SelectableManager.Register(props);
            _game.SelectableAction.OpenSelectable(selectableIndex);
            Func<bool> picked = () => isSelected;
            await picked.WaitToBeTrue();
        }
    }
}