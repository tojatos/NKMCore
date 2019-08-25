using System.Collections.Generic;
using System.Threading.Tasks;
using NKMCore.Extensions;

namespace NKMCore
{
    public class AllRandom : CharacterPick
    {
        public AllRandom(Game game, GamePreparerDependencies preparerDependencies) : base(game, preparerDependencies)
        {
        }

        public override Task BindCharactersToPlayers()
        {
            List<string> allCharacterNames = _preparerDependencies.Connection.GetCharacterNames();
            _game.Players.ForEach(p=>
            {
                while (p.Characters.Count != _preparerDependencies.NumberOfCharactersPerPlayer)
                {
                    string randomCharacterName = allCharacterNames.GetRandom();
                    allCharacterNames.Remove(randomCharacterName);
                    p.Characters.Add(CharacterFactory.Create(_game, randomCharacterName));
                }
            });
            return Task.CompletedTask;
        }
    }
}