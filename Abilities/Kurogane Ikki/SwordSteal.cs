using System.Collections.Generic;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Kurogane_Ikki
{
    public class SwordSteal : Ability, IClickable
    {
        public override string Name { get; } = "Sword Steal";
        protected override int Cooldown { get; } = 3;
        public override AbilityType Type { get; } = AbilityType.Normal;

        public Ability CopiedAbility;

        public SwordSteal(Game game) : base(game)
        {
            OnAwake += () =>
            {
                Validator.ToCheck.Add(()=>CopiedAbility!=null && CopiedAbility.CanBeUsed);
                ParentCharacter.AfterBeingHitByAbility += ability =>
                {
                    if(!(ability is IClickable)) return;
                    Ability a = AbilityFactory.CreateAndInit(ability.GetType(), Game);
                    if(a==null) return;
                    a.AfterUseFinish += Finish;

                    CopiedAbility = a;
                };
            };
        }

        public override List<HexCell> GetRangeCells() => CopiedAbility==null ? new List<HexCell>() : CopiedAbility.GetRangeCells();
        public override List<HexCell> GetTargetsInRange() => CopiedAbility==null ? new List<HexCell>() : CopiedAbility.GetTargetsInRange();

        public override string GetDescription()
        {
            string desc =
$@"{ParentCharacter.Name} kopiuje wrogą technikę, używając ostatnią umiejętność, która zadała mu obrażenia.
Czas odnowienia: {Cooldown}";
            if (CopiedAbility != null) desc += $"\n<i>Ostatnia skradziona technika: \n<b>{CopiedAbility.Name}</b></i>\n" + CopiedAbility.GetDescription();
            return desc;
        }

        public void Click() => ((IClickable) CopiedAbility)?.Click();
    }
}
