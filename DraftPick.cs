using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Templates;

namespace NKMCore
{
    public class DraftPick : CharacterPick
    {
        private readonly List<Character> _charactersToPick = Game.GetMockCharacters();
        public DraftPick(Game game, GamePreparerDependencies preparerDependencies) : base(game, preparerDependencies)
        {
        }

        public override async Task BindCharactersToPlayers()
        {
            if(_preparerDependencies.BansEnabled) await Bans();

            bool AllCharactersPicked() => _game.Players.All(p => p.Characters.Count == _preparerDependencies.NumberOfCharactersPerPlayer);
            while (!AllCharactersPicked())
            {
                foreach (GamePlayer player in _game.Players)
                    await DraftPickOneCharacter(player);
                if(AllCharactersPicked()) break;
                foreach (GamePlayer player in _game.Players.AsEnumerable().Reverse())
                    await DraftPickOneCharacter(player);
            }
        }

        private async Task Bans()
        {
            int bansLeft = _preparerDependencies.NumberOfBans;
            while(bansLeft != 0)
            {
                foreach (GamePlayer player in _game.Players)
                    await BanOneCharacter(player);
                bansLeft--;
                if(bansLeft==0) break;
                foreach (GamePlayer player in _game.Players.AsEnumerable().Reverse())
                    await BanOneCharacter(player);
                bansLeft--;
            }
        }

        private async Task DraftPickOneCharacter(GamePlayer player)
        {
            await SelectAndWait(new SelectableProperties
            {
                WhatIsSelected = SelectableProperties.Type.Character,
                IdsToSelect = _charactersToPick.Select(c => c.ID).ToList(),
                ConstraintOfSelection = SelectionConstraint.Single,
                OnSelectFinish = list =>
                {
                    Character picked = _charactersToPick.Single(c => c.ID == list[0]);
                    player.Characters.Add(CharacterFactory.Create(_game, picked.Name));
                    _charactersToPick.Remove(picked);
                },
                SelectionTitle = $"Wybór postaci - {player.Name}",
            });
        }
        private async Task BanOneCharacter(GamePlayer player)
        {
            await SelectAndWait(new SelectableProperties
            {
                WhatIsSelected = SelectableProperties.Type.Character,
                IdsToSelect = _charactersToPick.Select(c => c.ID).ToList(),
                ConstraintOfSelection = SelectionConstraint.Single,
                OnSelectFinish = list => _charactersToPick.RemoveAll(c => c.ID == list[0]),
                SelectionTitle = $"Banowanie postaci - {player.Name}",
            });
        }
    }
}