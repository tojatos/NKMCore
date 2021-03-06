﻿using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Aqua
{
    public class Purification : Ability, IClickable, IUseableCharacter
    {
        public override string Name { get; } = "Purification";
        protected override int Cooldown { get; } = 4;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int AbilityRange = 5;

        public Purification(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(AbilityRange);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() => $@"{ParentCharacter.Name} rzuca oczyszczający czar na sojusznika, zdejmując z niego wszelkie negatywne efekty.
Zasięg: {AbilityRange} Czas odnowienia: {Cooldown}";


        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            character.Effects.Where(e => e.Type == EffectType.Negative).ToList().ForEach(e => e.RemoveFromParent());
            Finish();
        }

    }
}
