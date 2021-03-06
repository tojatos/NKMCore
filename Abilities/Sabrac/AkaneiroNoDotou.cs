﻿using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sabrac
{
    public class AkaneiroNoDotou : Ability, IClickable, IUseableCell
    {
        public override string Name { get; } = "Akaneiro no Dotou";
        protected override int Cooldown { get; } = 5;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int Range = 9;
        private const int Damage = 30;
        private const int SilentDuration = 2;
        private const int Width = 3;
        public AkaneiroNoDotou(Game game) : base(game) { }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} tworzy falę płomieni i mieczy, na której przemieszcza się w wybrany punkt.
Fala zadaje {Damage} obrażeń magicznych i ucisza na {SilentDuration} tury wszystkich wrogów którzy zostali nią trafieni.

Zasięg: {Range}
Szerokość: {Width}
Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.IsFreeToStand);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(HexCell cell)
        {
            ParentCharacter.TryToTakeTurn();
            List<HexCell> targetCells = ParentCharacter.ParentCell.GetArea(cell, Width);
            ParentCharacter.MoveTo(cell);
            targetCells.WhereEnemiesOf(Owner).GetCharacters().ForEach(c =>
            {
                ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Magical));
                c.Effects.Add(new Silent(Game, SilentDuration, c, Name));
            });
            Finish();
        }
    }
}
