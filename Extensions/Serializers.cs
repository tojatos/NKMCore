using System.Collections.Generic;
using NKMCore.Templates;

namespace NKMCore.Extensions
{
    public static class Serializers
    {
//        public static List<GamePlayer> DeserializeGamePlayers(this string str) => new List<GamePlayer>();
//        public static string Serialize(this List<GamePlayer> gamePlayers) => "";
//        public static GamePlayer DeserializeGamePlayer(this string str) => new GamePlayer();
//        public static string Serialize(this GamePlayer gamePlayer) => "";
        public static GameType DeserializeGameType(this string str) => str.ToEnum<GameType>();
        public static string Serialize(this GameType gameType) => gameType.ToString();
        public static Character DeserializeCharacter(this string str) => new Character(new Character.Properties{Name = str});
        public static string Serialize(this Character character) => character.Name;
    }
}