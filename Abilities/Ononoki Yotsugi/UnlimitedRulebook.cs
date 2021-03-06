﻿using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ononoki_Yotsugi
{
    public class UnlimitedRulebook : Ability, IRunnable
    {
        public override string Name { get; } = "Unlimited Rulebook";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int TakenDamageDecreasePercent = 25;
        private const int Radius = 3;
        public UnlimitedRulebook(Game game) : base(game)
        {
            OnAwake += () => ParentCharacter.Abilities.ForEach(a => a.AfterUseFinish += Run);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Radius).AddOne(ParentCharacter.ParentCell);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() =>
$@"Po użyciu umiejętności przez {ParentCharacter.FirstName()},
ona i wszyscy sojusznicy dookoła niej będą otrzymywać obrażenia zmniejszone o {TakenDamageDecreasePercent}% w następnej fazie.

Promień: {Radius}";

        public void Run() => GetTargetsInRange().GetCharacters()
            .ForEach(c => c.Effects.Add(new TakenDamageModifier(Game, 1, -TakenDamageDecreasePercent, c, Name)));
    }
}
