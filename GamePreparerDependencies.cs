using System.Collections.Generic;
using NKMCore.Hex;

namespace NKMCore
{
    public class GamePreparerDependencies
    {
        public int NumberOfPlayers { get; set; }
        public List<string> PlayerNames { get; set; }
        public int NumberOfCharactersPerPlayer { get; set; }
        public bool BansEnabled { get; set; }
        public int NumberOfBans { get; set; }
        public HexMap HexMap { get; set; }
        public PickType PickType { get; set; }
        public GameType GameType { get; set; }
        public ISelectable Selectable { get; set; }
        public SelectableManager SelectableManager { get; set; }
        public SelectableAction SelectableAction { get; set; }
        public string LogFilePath { get; set; } //optional
    }
}