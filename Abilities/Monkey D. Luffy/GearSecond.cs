using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Monkey_D._Luffy
{
    public class GearSecond : Ability, IClickable, IEnableable
    {
        public override string Name { get; } = "Gear Second";
        protected override int Cooldown { get; } = 5;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int SpeedIncrease = 3;
        private const int Duration = 2;

        private int _currentDuration;
        public GearSecond(Game game) : base(game)
        {
            OnAwake += () => Active.Phase.PhaseFinished += () =>
            {
                if (!IsEnabled) return;
                _currentDuration++;
                if (_currentDuration <= Duration) return;

                IsEnabled = false;
                //TODO: check if current duration should not be set to 0
                Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Normal);
                if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = false;
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} przyśpiesza przepływ krwi w swoim ciele,
zwiększając swoją szybkość o {SpeedIncrease} i ulepszając swoją zwykłą umiejętność na {Duration} następne fazy.
Czas odnowienia: {Cooldown}";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration+1, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
            Ability normalAbility = ParentCharacter.Abilities.Find(a => a.Type == AbilityType.Normal);
            if (normalAbility is IEnchantable) ((IEnchantable) normalAbility).IsEnchanted = true;
            IsEnabled = true;
            Finish();
        }

        public bool IsEnabled { get; private set; }
    }
}
