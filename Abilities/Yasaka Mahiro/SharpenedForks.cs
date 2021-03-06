﻿using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Yasaka_Mahiro
{
    public class SharpenedForks : Ability, IClickable, IUseableCharacter
    {
        public override string Name { get; } = "Sharpened Forks";
        protected override int Cooldown { get; } = 3;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Damage = 5;
        private const float MissingHealthPercentDamage = 20;
        private const int Range = 7;

        private int _numberOfUses;

        public SharpenedForks(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
            AfterUseFinish += () => _numberOfUses = 0;
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} rzuca 3 naostrzone widelce,
zadając {Damage} + {MissingHealthPercentDamage}% brakującego zdrowia przewiwnika obrażeń fizycznych przy każdym trafieniu.
Każdy widelec może zostać wymierzony w innego wroga.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => PrepareFork();

        private void PrepareFork()
        {
            if (!CanBeUsed) Cancel();
            else Active.Prepare(this, GetTargetsInRange());
        }

        public void Use(Character targetCharacter)
        {
            ParentCharacter.TryToTakeTurn();
            int damageValue = (int) (Damage + MissingHealthPercentDamage / 100 *
                             (targetCharacter.HealthPoints.BaseValue - targetCharacter.HealthPoints.Value));
//          AnimationPlayer.Instance.StartCoroutine(
//              AnimationPlayer.Instance.SharpenedForkEnumerator(ParentCharacter.CharacterObject.transform,
//                  targetCharacter.CharacterObject.transform)); TODO: animation
            var damage = new Damage(damageValue, DamageType.Physical);
            ParentCharacter.Attack(this, targetCharacter, damage);
            _numberOfUses++;
            if (_numberOfUses < 3)
            {
                PrepareFork();
                return;
            }

            Finish();
        }
        public override void Cancel()
        {
            if (_numberOfUses == 0) OnFailedUseFinish();
            else Finish();
        }
    }
}
