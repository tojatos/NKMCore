using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Rem
{
    public class AlHuma : Ability, IClickable, IUseableCharacter
    {
        public override string Name { get; } = "Al Huma";
        protected override int Cooldown { get; } = 4;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Damage = 10;
        private const int Range = 7;

        public AlHuma(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} zamraża jednego wroga w zasięgu {Range} na jedną turę,
ogłuszając go i zadając {Damage} obrażeń magicznych.

Czas odnowiania: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(Character targetCharacter)
        {
            ParentCharacter.TryToTakeTurn();
            var damage = new Damage(Damage, DamageType.Magical);
            ParentCharacter.Attack(this,targetCharacter, damage);
            targetCharacter.Effects.Add(new Stun(Game, 1, targetCharacter, Name));
            Finish();
        }
    }
}
