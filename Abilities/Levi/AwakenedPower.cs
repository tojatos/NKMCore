using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Levi
{
    public class AwakenedPower : Ability, IEnableable
    {
        public override string Name { get; } = "Awakened Power";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int AttackBonus = 7;
        private const int SpeedBonus = 2;
        private const int HealthTresholdPercent = 25;
        public AwakenedPower(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.OnKill += TryEnabling;
                ParentCharacter.HealthPoints.StatChanged += (int1, int2) =>
                {
                    if (ParentCharacter.HealthPoints.Value <
                        ParentCharacter.HealthPoints.BaseValue * HealthTresholdPercent / 100f)
                        TryEnabling();
                };
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} otrzymuje na stałe {AttackBonus} ataku i {SpeedBonus} szybkości,
kiedy zabije wroga albo jego zdrowie spadnie poniżej {HealthTresholdPercent}%.
Efekt ten aktywuje się tylko raz.";

        public bool IsEnabled { get; private set; }

        private void TryEnabling()
        {
            if(IsEnabled) return;
            IsEnabled = true;
            ParentCharacter.AttackPoints.Value = ParentCharacter.AttackPoints.RealValue + AttackBonus;
            ParentCharacter.Speed.Value = ParentCharacter.Speed.RealValue + SpeedBonus;
        }
    }
}
