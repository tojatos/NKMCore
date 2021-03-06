﻿using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Levi
{
    public class VerticalManeuveringEquipment : Ability, IClickable, IUseableCell
    {
        public override string Name { get; } = "Vertical Maneuvering Equipment";
        protected override int Cooldown { get; } = 2;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Range = 7;
        private const int MoveTargetRange = 7;

        public event Delegates.CharacterCell OnSwing;

        public VerticalManeuveringEquipment(Game game) : base(game)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.Type == HexCell.TileType.Wall);
        private List<HexCell> GetMoveTargets(HexCell cell) =>
            cell.GetNeighbors(Owner, MoveTargetRange, SearchFlags.StraightLine).FindAll(e => e.IsFreeToStand);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} zaczepia się ściany w zasięgu {Range} i przemieszcza się o max. {MoveTargetRange} pól.";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(HexCell cell)
        {
            if (cell.Type == HexCell.TileType.Wall)
            {
                Active.Prepare(this, GetMoveTargets(cell));
                OnSwing?.Invoke(ParentCharacter, cell);
            }
            else
            {
                ParentCharacter.TryToTakeTurn();
                ParentCharacter.MoveTo(cell);
                Finish();
            }
        }

        public override void Cancel()
        {
            base.Cancel();
            OnSwing?.Invoke(ParentCharacter, ParentCharacter.ParentCell);
        }
    }
}
