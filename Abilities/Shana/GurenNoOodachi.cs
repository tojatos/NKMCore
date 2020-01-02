using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Shana
{
    public class GurenNoOodachi : Ability, IClickable
    {
        public override string Name { get; } = "Guren no Oodachi";
        protected override int Cooldown { get; } = 5;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int AttackIncrease = 5;
        private const int BasicAttackRangeIncrease = 4;
        private const int Duration = 3;

        public GurenNoOodachi(Game game) : base(game){}
        public override string GetDescription()
        {
            return $@"Zwiększa atak o {AttackIncrease}, oraz zasięg ataków o {BasicAttackRangeIncrease}
Czas trwania: {Duration}    Czas odnowienia: {Cooldown}";
        }

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration, AttackIncrease, ParentCharacter, StatType.AttackPoints, Name));
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration, BasicAttackRangeIncrease, ParentCharacter, StatType.BasicAttackRange, Name));
            Finish();

        }
    }
}
