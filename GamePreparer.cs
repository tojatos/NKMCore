using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NKMCore.Extensions;
using NKMCore.Templates;

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
                Connection = dependencies.Connection,
                LogFilePath = dependencies.LogFilePath,
                PlaceAllCharactersRandomlyAtStart = dependencies.PickType == PickType.AllRandom,
            };
            _gameDependenciesValidator = new GameDependenciesValidator(
                _gameDependencies,
                dependencies.NumberOfPlayers,
                dependencies.NumberOfCharactersPerPlayer
            );
        }
        
		private List<GamePlayer> GetPlayers()
        {
            if (_preparerDependencies.GameType == GameType.Local)
            {
                return Enumerable.Range(0, _preparerDependencies.NumberOfPlayers)
                    .Select(n => new GamePlayer {Name = $"Player {n}"}).ToList();
            }
            return new List<GamePlayer>();
        }

        public bool AreOptionsValid => _gameDependenciesValidator.AreOptionsValid;

        public async Task<Game> CreateGame()
        {
	        var game = new Game(_gameDependencies);
            CharacterPick characterPick = CharacterPick.Create(game, _preparerDependencies);
            await characterPick.BindCharactersToPlayers();
            return game;
        }
    }
}