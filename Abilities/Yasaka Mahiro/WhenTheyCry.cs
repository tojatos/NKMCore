using System.Collections.Generic;
using NKMCore.Templates;

namespace NKMCore.Abilities.Yasaka_Mahiro
{
    public class WhenTheyCry : Ability
    {
        public override string Name { get; } = "When They Cry";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int AdditionalDamagePercent = 25;
        private readonly List<Character> _damagedCharacters = new List<Character>();
        public WhenTheyCry(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.BeforeAttack += (character, damage) =>
                {
                    if (_damagedCharacters.Contains(character))
                    {
                        damage.Value += damage.Value * AdditionalDamagePercent / 100;
                    }
                };
                ParentCharacter.AfterAttack += (character, damage) =>
                {
                    if (damage.Value <= 0) return;
                    if (character.Owner != Active.GamePlayer && !_damagedCharacters.Contains(character))
                        _damagedCharacters.Add(character);
                };
            };
        }
        public override string GetDescription() =>
$"{ParentCharacter.Name} zadaje dodatkowe {AdditionalDamagePercent}% obrażeń zranionym wcześniej wrogom.";
    }
}
