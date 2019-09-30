using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Kirito
{
    public class Switch : Ability, IClickable, IUseableCharacter
    {
        private const int Range = 7;

        public Switch(Game game) : base(game, AbilityType.Normal, "Switch", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
            CanUseOnGround = false;
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange()
        {
            List<HexCell> targetCandidates = GetRangeCells().WhereFriendsOf(Owner);
            List<Character> enemiesOnMap = HexMap.Cells.WhereEnemiesOf(Owner).GetCharacters();
            List<HexCell> enemyAttackRanges = enemiesOnMap.SelectMany(enemy => enemy.GetBasicAttackCells()).ToList();
            return enemyAttackRanges.Contains(ParentCharacter.ParentCell) ? targetCandidates : targetCandidates.Intersect(enemyAttackRanges).ToList();
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} zamienia się miejscami z wybranym sojusznikiem,
jeśli któryś z nich znajduje się w zasięgu podstawowego ataku wrogiej postaci.
{ParentCharacter.Name} może użyć podstawowego ataku albo super umiejętności zaraz po użyciu tej umiejętności.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();
            HexMap.Swap(ParentCharacter, character);
            ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
            ParentCharacter.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = true; // TODO

            void OnAttack(Character c, Damage d)
            {
                ParentCharacter.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
                ParentCharacter.AfterBasicAttack -= OnAttack;
            }

            ParentCharacter.AfterBasicAttack += OnAttack;

            void OnAbilityUse(Ability ability)
            {
                ParentCharacter.HasFreeAttackUntilEndOfTheTurn = false;
                ParentCharacter.AfterAbilityUse -= OnAbilityUse;
            }

            ParentCharacter.AfterAbilityUse += OnAbilityUse;

            void OnMove(List<HexCell> cellList)
            {
                ParentCharacter.HasFreeUltimatumAbilityUseUntilEndOfTheTurn = false;
                ParentCharacter.HasFreeAttackUntilEndOfTheTurn = false;
                ParentCharacter.AfterBasicMove -= OnMove;
            }

            ParentCharacter.AfterBasicMove += OnMove;

            Active.Turn.TurnFinished += character1 =>
            {
                if (character1 != ParentCharacter) return;
                ParentCharacter.AfterBasicAttack -= OnAttack;
                ParentCharacter.AfterAbilityUse -= OnAbilityUse;
                ParentCharacter.AfterBasicMove -= OnMove;
            };
            Finish();
        }
    }
}