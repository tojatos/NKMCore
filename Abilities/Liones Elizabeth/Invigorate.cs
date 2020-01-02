using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Liones_Elizabeth
{
    public class Invigorate : Ability, IClickable, IUseableCharacter
    {
        public override string Name { get; } = "Invigorate";
        protected override int Cooldown { get; } = 3;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Range = 5;
        private const int Heal = 6;
        private const int Duration = 3;
        public Invigorate(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} nakłada na sojusznika zaklęcie, które leczy go o {Heal} przez {Duration} fazy.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            character.Effects.Add(new HealOverTime(Game, ParentCharacter, Heal, Duration, character, Name));
            Finish();
        }
    }
}
