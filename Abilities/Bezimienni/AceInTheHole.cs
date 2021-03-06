﻿using NKMCore.Templates;

namespace NKMCore.Abilities.Bezimienni
{
    public class AceInTheHole : Ability
    {
        public override string Name { get; } = "Ace in the hole";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int HpPercentActivate = 50;
        private int _damageThisTurn;
        public bool HasFreeAbility { get; set; }

        public AceInTheHole(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.AfterBeingDamaged += damage =>
                {
                    if (HasFreeAbility) return;
                    _damageThisTurn += damage.Value;
                    if (_damageThisTurn > ParentCharacter.HealthPoints.Value * (HpPercentActivate / 100f))
                        HasFreeAbility = true;
                };
                Active.Turn.TurnFinished += character => _damageThisTurn = 0;
            };
        }

        public override string GetDescription() =>
$@"Jeśli Bezimienni otrzymają na raz (podczas ruchu jednej postaci) obrażenia wynoszące więcej niż 40% ich maksymalnego HP,
będą oni mogli w swoim następnym ruchu użyć jednej ze swoich umiejętności, niezależnie od jej CD.
{Name} nie wpływa na liczenie faktycznego CD umiejętności.";

    }
}
