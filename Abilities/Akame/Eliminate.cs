using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Akame
{
    public class Eliminate : Ability, IClickable, IUseableCharacter
    {
        private const int Range = 3;
        public Eliminate(Game game) : base(game, AbilityType.Normal, "Eliminate", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} trafia krytycznie zadając podwójne obrażenia.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            var dmg = new Damage(ParentCharacter.AttackPoints.Value*2, DamageType.Physical);
            ParentCharacter.Attack(this, character, dmg);
            Finish();
        }
    }
}