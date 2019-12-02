using System.Collections.Generic;
using System.Data;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public static class NKMData
    {
        public static IDbConnection Connection;
        private static readonly List<string> DisabledCharacters = new List<string>
        {
            "Yoshino",
        };
        public static List<string> GetCharacterNames() => Connection.Select("SELECT Name FROM Character")
                                                          .SelectMany(row => row.Values).Except(DisabledCharacters)
                                                          .ToList();

        public static IEnumerable<string> GetAbilityClassNames(string characterName) => Connection.Select(
$@"SELECT Ability.ClassName AS AbilityName FROM Character 
INNER JOIN Character_Ability ON Character.ID = Character_Ability.CharacterID
INNER JOIN Ability ON Ability.ID = Character_Ability.AbilityID
WHERE Character.Name = '{characterName}';")
           .SelectMany(row => row.Values).ToList();

        public static Dictionary<string, string> GetCharacterData(string characterName) => Connection.Select(
$@"SELECT AttackPoints, HealthPoints, BasicAttackRange, Speed, PhysicalDefense, MagicalDefense, FightType, Description, Quote, Author.Name
FROM Character INNER JOIN Author ON Character.AuthorID = Author.ID
WHERE Character.Name = '{characterName}';")[0];

        /// <summary>
        /// Get a copy of every character in the game
        /// </summary>
        public static List<Character> GetMockCharacters()
        {
            List<string> names = GetCharacterNames();
            var toReturn = new List<Character>();
            int len = names.Count;
            for (int i = 1; i <= len; ++i)
            {
                toReturn.Add(CharacterFactory.Create(null, names[i-1], -i));
            }

            return toReturn;
        }

        public static string GetInfos(IEnumerable<Character> characters) => string.Join(
            "\n\n",
            characters.Select(c => c.Name + '\n' +
                                   string.Join(
                                       "\n\n",
                                       c.Abilities.Select(
                                           a => a.Name + '\n' +
                                                a.GetDescription()))));

    }
}