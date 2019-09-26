using System;
using System.Collections.Generic;
using System.Linq;

namespace NKMCore.Extensions
{
    public static class Serializers
    {
        public static GameType DeserializeGameType(this string str) => str.ToEnum<GameType>();
        public static string Serialize(this GameType gameType) => gameType.ToString();
        public static PickType DeserializePickType(this string str) => str.ToEnum<PickType>();
        public static string Serialize(this PickType pickType) => pickType.ToString();
        private const string Marker = "|MARKER|";
        public static GamePreparerDependencies DeserializeGamePreparerDependencies(this string str)
        {
            string[] parts = str.Split(new[] {Marker}, StringSplitOptions.None);
            return new GamePreparerDependencies
            {
                NumberOfPlayers = int.Parse(parts[0]),
                PlayerNames = new List<string>(parts[1].Split(';')),
                NumberOfCharactersPerPlayer = int.Parse(parts[2]),
                BansEnabled = bool.Parse(parts[3]),
                NumberOfBans = int.Parse(parts[4]),
                HexMap = HexMapSerializer.Deserialize(parts[5]),
                PickType = parts[6].DeserializePickType(),
                GameType = parts[7].DeserializeGameType(),
            };
        }

        public static string Serialize(this GamePreparerDependencies deps)
        {
            if(deps.PlayerNames.Any(n => n.Contains(';')))
                throw new FormatException("Commas are disallowed in player names!");

            string[] parts = {
                deps.NumberOfPlayers.ToString(),
                string.Join(";", deps.PlayerNames),
                deps.NumberOfCharactersPerPlayer.ToString(),
                deps.BansEnabled.ToString(),
                deps.NumberOfBans.ToString(),
                HexMapSerializer.Serialize(deps.HexMap),
                deps.PickType.Serialize(),
                deps.GameType.Serialize(),
            };

            return string.Join(Marker, parts);
        }
    }
}
