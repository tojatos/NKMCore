using System.Collections.Generic;
using NKMCore.Hex;

namespace NKMCore
{
    public class GameDependencies
    {
        public List<GamePlayer> Players { get; set; }
        public HexMap HexMap { get; set; }
        public GameType Type { get; set; }
        public bool PlaceAllCharactersRandomlyAtStart { get; set; }
        public ISelectable Selectable { get; set; }
        public SelectableManager SelectableManager { get; set; }
        public SelectableAction SelectableAction { get; set; }
        public ILogger Logger { get; set; }
    }

    public enum GameType
    {
        Undefined,
        Local,
        Multiplayer,
        Replay,
    }
}