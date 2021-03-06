﻿using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Itsuka_Kotori
{
    public class Conflagration : Ability, IClickable, IUseableCellList
    {
        public override string Name { get; } = "Conflagration";
        protected override int Cooldown { get; } = 2;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int Range = 10;
        private const int Radius = 3;
        private const int DamagePercent = 50;

        public Conflagration(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.BeforeBasicAttack += (character, damage) =>
                {
                    bool isEnemyOnConflagration = character.ParentCell.Effects.ContainsType(typeof(HexCellEffects.Conflagration));
                    bool isEnemyInBasicAttackRange = ParentCharacter.DefaultGetBasicAttackCells().GetCharacters().Contains(character);
                    if (!isEnemyInBasicAttackRange && isEnemyOnConflagration) damage.Value = (int) (damage.Value * (DamagePercent / 100f));
                };
                ParentCharacter.GetBasicAttackCells = () =>
                {
                    List<HexCell> cellRange = ParentCharacter.DefaultGetBasicAttackCells();
                    IEnumerable<HexCell> cellsWithConflagrationAndEnemyCharacters = HexMap.Cells
                        .Where(c => c.Effects.Any(e => e.GetType() == typeof(HexCellEffects.Conflagration)))
                        .ToList().WhereEnemiesOf(Owner);
                    cellRange.AddRange(cellsWithConflagrationAndEnemyCharacters);
                    return cellRange.Distinct().ToList();
                };
            };
        }

        public override string GetDescription() => string.Format(
@"{0} wywołuje Pożar na wskazanym obszarze o public {3}.
{0} może atakować wrogów znajdujących się na terenie Pożaru podstawowymi atakami, zadając 50% zwykłych obrażeń, niezależnie od tego gdzie sama się znajduje.

Zasięg: {1}    Czas odnowienia: {2}",
            ParentCharacter.Name, Range, Cooldown, Radius);

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public void Click() => Active.PrepareAirSelection(this, GetRangeCells(), AirSelection.SelectionShape.Circle, Radius);
        public void Use(List<HexCell> cells)
        {
            ParentCharacter.TryToTakeTurn();
            cells.ForEach(c => c.Effects.Add(new HexCellEffects.Conflagration(Game, -1, c, ParentCharacter)));
            Finish();
        }
    }
}
