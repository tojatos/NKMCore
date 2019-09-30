using System;
using NKMCore.Templates;

namespace NKMCore.Effects
{
    public class TakenDamageModifier : Effect
    {
        private readonly int _value;
        //Increase taken damage by value
        public TakenDamageModifier(Game game, int cooldown, int value, Character parentCharacter, string name = null) : base(game, cooldown, parentCharacter, name)
        {
            Name = name ?? "Taken Damage Modifier";
            _value = value;
            Type = value <= 0 ? EffectType.Positive : EffectType.Negative;
            void DamageD(Damage damage) => damage.Value += (int) (damage.Value * (_value / 100f));
            parentCharacter.BeforeBeingDamaged += DamageD;
            OnRemove += () => parentCharacter.BeforeBeingDamaged -= DamageD;
        }
        public override string GetDescription() =>
$@"{(_value > 0 ? "Zwiększa" : "Zmniejsza")} otrzymywane obrażenia o {Math.Abs(_value)}%
Czas do zakończenia efektu: {CurrentCooldown}";
    }
}
