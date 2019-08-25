using System.Collections.Generic;
using System.Linq;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Bezimienni
{
    public class Check : Ability, IClickable, IUseableCharacter
    {
        public Check(Game game) : base(game, AbilityType.Normal, "Check", 2)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }
        public override string GetDescription() =>
            $@"Bezimienni szachują wybranego przeciwnika, wymuszając jego ruch.\nSzachowany wróg nie może użyć podstawowego ataku.

Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMap.Cells);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner).Where(c => c.FirstCharacter.TookActionInPhaseBefore == false).ToList();

        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(Character character)
        {
            ParentCharacter.TryToTakeTurn();

            void ForceAction(GamePlayer player)
            {
                if (player != character.Owner) return;
                Active.Turn.CharacterThatTookActionInTurn = character;
                Active.Turn.TurnStarted -= ForceAction;
            }

            Active.Turn.TurnStarted += ForceAction;
            character.Effects.Add(new Disarm(Game, 1, character, Name));
            Finish();
        }

    }
}
