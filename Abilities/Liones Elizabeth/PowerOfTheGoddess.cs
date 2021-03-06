﻿using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Liones_Elizabeth
{
    public class PowerOfTheGoddess : Ability, IClickable
    {
        public override string Name { get; } = "Power of the goddess";
        protected override int Cooldown { get; } = 6;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int Heal = 20;
        public PowerOfTheGoddess(Game game) : base(game){}

        public override List<HexCell> GetRangeCells() => HexMap.Cells;
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} leczy wszystkich sojuszników (w tym siebie) na mapie za {Heal} HP.

Czas odnowienia: {Cooldown}";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            GetTargetsInRange().GetCharacters().ForEach(c => ParentCharacter.Heal(c, Heal));
            Finish();
        }
    }
}
