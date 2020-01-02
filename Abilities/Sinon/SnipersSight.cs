using System;
using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sinon
{
    public class SnipersSight : Ability
    {
        public override string Name { get; } = "Sniper's Sight";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        public SnipersSight(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.GetBasicAttackCells = GetBasicAttackCellsOverride;
            };
        }

        public override string GetDescription() => "Zasięg ataków podstawowych tej postaci jest kulisty.";
        public override List<HexCell> GetRangeCells() => ParentCharacter.GetBasicAttackCells();
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        private List<HexCell> GetBasicAttackCellsOverride()
        {
            switch (ParentCharacter.Type)
            {
                case FightType.Ranged:
                    return GetNeighboursOfOwner(ParentCharacter.BasicAttackRange.Value);
                case FightType.Melee:
                    return GetNeighboursOfOwner(ParentCharacter.BasicAttackRange.Value, SearchFlags.StopAtWalls);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
