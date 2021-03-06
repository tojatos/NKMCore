﻿using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Kirito
{
    public class StarburstStream : Ability, IClickable, IEnableable, IUseableCharacter
    {
        public override string Name { get; } = "Starburst Stream";
        protected override int Cooldown { get; } = 6;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int Range = 3;
        private const int AttackTimes = 16;
        private const int Damage = 2;
        public StarburstStream(Game game) : base(game)
        {
            OnAwake += () =>
            {
                Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
                ParentCharacter.AfterBasicAttack += (character, damage) =>
                {
                    if(!IsEnabled) return;
                    if (_gotFreeAttackThisTurn) return;
                    ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
                    _gotFreeAttackThisTurn = true;
                };
                Active.Turn.TurnStarted += player => _gotFreeAttackThisTurn = false;
            };
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} atakuje przeciwnika {AttackTimes} razy.
Każdy cios zadaje {Damage} pkt obrażeń nieuchronnych.
Po użyciu tej umiejętnoości Kirito może atakować 2 razy na turę.
Efekt jest trwały.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        private bool _gotFreeAttackThisTurn;

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            for (int i = 0; i < AttackTimes; i++)
                ParentCharacter.Attack(this, character, new Damage(Damage, DamageType.True));
            if (!IsEnabled) IsEnabled = true;
            Finish();
        }

        public bool IsEnabled { get; private set; }
    }
}
