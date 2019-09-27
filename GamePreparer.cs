using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;

namespace NKMCore
{
    /// <summary>
    /// This class is responsible for creation and validation of dependencies of the Game.
    /// </summary>
    public class GamePreparer
    {
        private readonly GamePreparerDependencies _preparerDependencies;
        private readonly GameDependencies _gameDependencies;
        private readonly GameDependenciesValidator _gameDependenciesValidator;

        public GamePreparer(GamePreparerDependencies dependencies)
        {
            _preparerDependencies = dependencies;
            _gameDependencies = new GameDependencies
            {
                Players = GetPlayers(),
                HexMap = dependencies.HexMap,
                Type = dependencies.GameType,
                Selectable = dependencies.Selectable,
                SelectableAction = dependencies.SelectableAction,
                SelectableManager = dependencies.SelectableManager,
                Logger = dependencies.Logger,
                PlaceAllCharactersRandomlyAtStart = dependencies.PickType == PickType.AllRandom,
            };
            _gameDependenciesValidator = new GameDependenciesValidator(
                _gameDependencies,
                dependencies.NumberOfPlayers,
                dependencies.NumberOfCharactersPerPlayer
            );
        }

        private List<GamePlayer> GetPlayers() => _preparerDependencies.PlayerNames.Select(name => new GamePlayer{ Name = name}).ToList();

        public bool AreOptionsValid => _gameDependenciesValidator.AreOptionsValid;

        public async Task<Game> CreateGame()
        {
            _gameDependencies.Logger.Log("# START: Logging game preparer dependencies");
            _gameDependencies.Logger.Log(_preparerDependencies.Serialize());
            _gameDependencies.Logger.Log("# FINISH: Logging game preparer dependencies");

            var game = new Game(_gameDependencies);
            CharacterPick characterPick = CharacterPick.Create(game, _preparerDependencies);
            await characterPick.BindCharactersToPlayers();

            _gameDependencies.Logger.Log("# START: Logging characters");
            _gameDependencies.Logger.Log(_gameDependencies.Players.SerializeCharacters());
            _gameDependencies.Logger.Log("# FINISH: Logging characters");

            return game;
        }
    }
}