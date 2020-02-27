using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Extensions
{
    public static class Serializers
    {
        public static GameType DeserializeGameType(this string str) => str.ToEnum<GameType>();
        public static string Serialize(this GameType gameType) => gameType.ToString();
        public static PickType DeserializePickType(this string str) => str.ToEnum<PickType>();
        public static string Serialize(this PickType pickType) => pickType.ToString();

        /// <summary>
        /// Used to split GamePreparerDependencies
        /// </summary>
        private const string Marker = "\n|MARKER|\n";

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

        public static string SerializeCharacters(this IEnumerable<GamePlayer> players) =>
            string.Join("\n\n", players.Select(p => string.Join("\n", p.Characters.Select(c => c.Name + '|' + c.ID))));

        public static void DeserializeCharactersAndInsertIntoGame(this string str, Game game)
        {
            Character[][] characterList = str.Split(new[] {"\n\n"}, StringSplitOptions.None).Select(s => s.Split('\n').Select(c =>
            {
                string[] data = c.Split('|');
                string characterName = data[0];
                int characterID = int.Parse(data[1]);
                return CharacterFactory.Create(game, characterName, characterID);
            }).ToArray()).ToArray();
            int playersCount = game.Players.Count;

            for (int i = 0; i < playersCount; ++i)
            {
                game.Players[i].Characters.AddRange(characterList[i]);
            }
        }
    }
}
