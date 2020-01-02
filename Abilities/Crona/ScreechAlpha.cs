using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Crona
{
    public class ScreechAlpha : Ability, IClickable
    {
        public override string Name { get; } = "Screech Alpha";
        protected override int Cooldown { get; } = 4;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Radius = 3;

        public ScreechAlpha(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Radius);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
            @"Miecz Crony, Ragnarok, wydaje z siebie krzyk,
ogłuszający wrogów dookoła na 1 turę i spowalniający ich na 1 następną.";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            GetTargetsInRange().GetCharacters().ForEach(c =>
            {
                c.Effects.Add(new Stun(Game, 1, c, Name));
                c.Effects.Add(new StatModifier(Game, 2, -3, c, StatType.Speed, Name));
            });
            Finish();
        }
    }
}
