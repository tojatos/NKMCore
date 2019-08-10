using System.Linq;

namespace NKMCore
{
    public class GameDependenciesValidator
    {
        private readonly GameDependencies _gameDependencies;
        private readonly int _numberOfPlayers;
        private readonly int _numberOfCharacters;
        
        public GameDependenciesValidator(GameDependencies dependencies, int numberOfPlayers, int numberOfCharacters)
        {
            _gameDependencies = dependencies;
            _numberOfPlayers = numberOfPlayers;
            _numberOfCharacters = numberOfCharacters;
        }
        
        private bool NumberOfPlayersCorrect => 
            _gameDependencies.Players.Count == _numberOfPlayers;
        private bool NumberOfCharactersPerPlayerCorrect =>
            _gameDependencies.Players.All(p => p.Characters.Count == _numberOfCharacters);
        private bool IsHexMapSet =>
            _gameDependencies.HexMap != null;
        private bool IsGameTypeSet =>
            _gameDependencies.Type != GameType.Undefined;
        private bool IsSelectableSet => 
            _gameDependencies.Selectable != null;
        private bool IsConnectionSet =>
            _gameDependencies.Connection != null;

        public bool AreOptionsValid =>
            NumberOfPlayersCorrect &&
            NumberOfCharactersPerPlayerCorrect &&
            IsHexMapSet &&
            IsGameTypeSet &&
            IsSelectableSet &&
            IsConnectionSet;
        
    }
}