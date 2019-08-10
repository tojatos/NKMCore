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
			await SelectAndWait(new SelectableProperties<Character>
			{
				ToSelect = _charactersToPick,
				ConstraintOfSelection = list => list.Count == 1,
				OnSelectFinish = list =>
				{
					player.Characters.Add(CharacterFactory.Create(_game, list[0].Name));
					_charactersToPick.Remove(list[0]);
				},
				SelectionTitle = $"Wybór postaci - {player.Name}",
			});
		}
		private async Task BanOneCharacter(GamePlayer player)
		{
			await SelectAndWait(new SelectableProperties<Character>
			{
				ToSelect = _charactersToPick,
				ConstraintOfSelection = list => list.Count == 1,
				OnSelectFinish = list => _charactersToPick.Remove(list[0]),
				SelectionTitle = $"Banowanie postaci - {player.Name}",
			});
		}
    }
}