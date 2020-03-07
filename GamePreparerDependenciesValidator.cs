namespace NKMCore
{
    public class GamePreparerDependenciesValidator
    {
        private readonly GamePreparerDependencies _gamePreparerDependencies;

        public GamePreparerDependenciesValidator(GamePreparerDependencies dependencies)
        {
            _gamePreparerDependencies = dependencies;
        }

        private bool IsHexMapSet =>
            _gamePreparerDependencies.HexMap != null;
        private bool NumberOfPlayersCorrect =>
            _gamePreparerDependencies.NumberOfPlayers <= _gamePreparerDependencies.HexMap.MaxPlayers;

        public bool AreOptionsValid =>
            IsHexMapSet &&
            NumberOfPlayersCorrect;
        //TODO: check number of characters per player depending on map, to be done after getting rid of map scriptables
    }
}