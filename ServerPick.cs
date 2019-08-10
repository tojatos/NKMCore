using System.Threading.Tasks;

namespace NKMCore
{
    public class ServerPick : CharacterPick
    {
        public ServerPick(Game game, GamePreparerDependencies preparerDependencies) : base(game, preparerDependencies) { }

        public override Task BindCharactersToPlayers() => Task.CompletedTask;
    }
}