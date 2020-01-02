using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sabrac
{
    public class Fuiuchi : Ability, IClickable, IRunnable
    {
        public override string Name { get; } = "Fuiuchi";
        protected override int Cooldown { get; } = 3;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Range = 3;
        private const int Damage = 15;
        private const int SlowDuration = 2;
        private const int SlowTo = 3;
        public Fuiuchi(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} przywołuje słup płomieni, zadając {Damage} obrażeń magicznych
i spowalniając trafionych wrogów do {SlowTo}.

Zasięg: {Range}
Czas trwania spowolnienia: {SlowDuration}
Czas odnowienia: {Cooldown}";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            Run();
        }

        public void Run()
        {
            GetTargetsInRange().GetCharacters().ForEach(c =>
            {
                ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Magical));
                c.Effects.Add(new StatModifier(Game, SlowDuration, -c.Speed.Value+SlowTo, c, StatType.Speed, Name));
            });
            Finish();
        }
    }
}
