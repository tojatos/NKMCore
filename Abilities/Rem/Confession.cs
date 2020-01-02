using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Rem
{
    public class Confession : Ability, IClickable, IUseableCharacter
    {
        public override string Name { get; } = "Confession";
        protected override int Cooldown { get; } = 4;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int Range = 6;

        public Confession(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner).FindAll(c => c.FirstCharacter.TookActionInPhaseBefore);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} wyznaje miłość wybranej postaci, umożliwiając jej ponowną akcję w tej fazie.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            character.Refresh();
            Finish();
        }
    }
}
