using System.Collections.Generic;
using NKMCore.Templates;

namespace NKMCore.Extensions
{
    public static class Serializers
    {
        public static GameType DeserializeGameType(this string str) => str.ToEnum<GameType>();
        public static string Serialize(this GameType gameType) => gameType.ToString();
        public static PickType DeserializePickType(this string str) => str.ToEnum<PickType>();
        public static string Serialize(this PickType pickType) => pickType.ToString();
    }
}