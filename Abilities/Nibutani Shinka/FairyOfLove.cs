﻿using NKMCore.Templates;

namespace NKMCore.Abilities.Nibutani_Shinka
{
    public class FairyOfLove : Ability, IClickable, IEnableable
    {
        public override string Name { get; } = "Fairy of Love";
        protected override int Cooldown { get; } = 6;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int Duration = 3;
        private int _currentDuration;
        public FairyOfLove(Game game) : base(game)
        {
            OnAwake += () => Active.Phase.PhaseFinished += () =>
            {
                if (!IsEnabled) return;
                _currentDuration++;
                if (_currentDuration <= Duration) return;

                IsEnabled = false;
                Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Passive);
                if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = false;
            };
        }

        public override string GetDescription()
        {
            string desc =
$@"{ParentCharacter.Name} wzmacnia swoją umiejętność bierną.

Czas trwania: {Duration}    Czas odnowienia: {Cooldown}";
            if (IsEnabled) desc += $"\n<i>To jest {_currentDuration} faza działania tej umiejętności</i>";
            return desc;
        }

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Passive);
            if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = true;
            IsEnabled = true;
            _currentDuration = 1;
            Finish();
        }

        public bool IsEnabled { get; set; }
    }
}
