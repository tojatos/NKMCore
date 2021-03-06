﻿using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Satou_Kazuma
{
    public class DrainTouch : Ability, IClickable, IUseableCharacter
    {
        public override string Name { get; } = "Drain Touch";
        protected override int Cooldown { get; } = 3;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Damage = 18;
        private const int Range = 6;
        public DrainTouch(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} wysysa z przeciwnika {Damage} HP,
zadając mu tyle obrażeń magicznych
i przywracając sobie HP równe wartości zadanej przeciwnikowi.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            var dmg = new Damage(Damage, DamageType.Magical);
            ParentCharacter.Attack(this, character, dmg);
            ParentCharacter.Heal(ParentCharacter, dmg.Value);
            Finish();
        }
    }
}
