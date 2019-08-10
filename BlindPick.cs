using System.Collections.Generic;
using System.Threading.Tasks;
using NKMCore.Templates;

namespace NKMCore
{
    public class BlindPick : CharacterPick
    {
        public BlindPick(Game game, GamePreparerDependencies preparerDependencies) : base(game, preparerDependencies)
        {
        }

        public override async Task BindCharactersToPlayers()
        {
			foreach (GamePlayer p in _game.Players)
			{
				List<Character> allCharacters = Game.GetMockCharacters();
				await PickCharacters(allCharacters, _preparerDependencies.NumberOfCharactersPerPlayer, p, _game);
			}
        }
    }
}