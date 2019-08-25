using System.Linq;
using NKMCore.Templates;

namespace NKMCore.Extensions
{
    public static class CharacterExtension
    {
        public static bool IsEnemyFor(this Character character, GamePlayer player) => character.Owner != player;
        public static bool IsEnemyFor(this Character character, Character other) => character.IsEnemyFor(other.Owner);
        public static string FormattedFirstName(this Character character) => string.Format("<color={0}><</color><b>{1}</b><color={0}>></color>", character.Owner.GetColor(character.Game), character.Name.Split(' ').Last());

        private static readonly string[] Colors =
        {
            "#FF0000",
            "#00FF00",
            "#0000FF",
            "#00FFFF",
        };

        public static string GetColor(this GamePlayer gamePlayer, Game game) => Colors[gamePlayer.GetIndex(game)];
    }
}