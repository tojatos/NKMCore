using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Gilgamesh
{
    public class TheFistHero : Ability
    {
        public override string Name { get; } = "The Fist Hero";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int DamageReductionPercent = 10;
        private const int AdditionalDamagePercent = 10;
        public TheFistHero(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.BeforeAttack += (character, damage) =>
                {
                    int modifier = ParentCharacter.Effects.ContainsType(typeof(PassiveBuff)) ? 2 : 1;
                    damage.Value += modifier * damage.Value * AdditionalDamagePercent / 100;
                };
                ParentCharacter.BeforeBeingDamaged += damage =>
                {
                    int modifier = ParentCharacter.Effects.ContainsType(typeof(PassiveBuff)) ? 2 : 1;
                    damage.Value -= modifier * damage.Value * DamageReductionPercent / 100;
                };
            };
        }
        public override string GetDescription() =>
$@"Dzięki nieznającemu kresu skarbcowi, {ParentCharacter.Name} jest w stanie znaleźć odpowiedź na każdego wroga.
W walce otrzymuje on {DamageReductionPercent}% mniej obrażeń, a jego ataki i umiejętności zadają dodatkowe {AdditionalDamagePercent}% obrażeń.";
    }
}
