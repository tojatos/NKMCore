using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Akame
{
    public class LittleWarHorn : Ability, IClickable, IEnableable
    {
        public override string Name { get; } = "Little War Horn";
        protected override int Cooldown { get; } = 7;
        public override AbilityType Type { get; } = AbilityType.Ultimatum;

        private const int SpeedIncrease = 4;
        private const int AttackIncrease = 10;
        private const int Duration = 5;
        private const int DefaultSpeed = 5;

        private int _currentDuration;
        public LittleWarHorn(Game game) : base(game)
        {
            OnAwake += () => Active.Phase.PhaseFinished += () =>
            {
                if (!IsEnabled) return;
                _currentDuration++;
                if (_currentDuration <= Duration) return;

                IsEnabled = false;
                _currentDuration = 0;
                ParentCharacter.Speed.Value = DefaultSpeed;
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} zyskuje {AttackIncrease} do ataku, {SpeedIncrease} do ruchu na {Duration} tur.
Po zakończeniu efektu jej prędkość zostaje ustawiona na {DefaultSpeed}.
Czas odnowienia: {Cooldown}";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration + 1, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration + 1, AttackIncrease, ParentCharacter, StatType.AttackPoints, Name));
            IsEnabled = true;
            Finish();
        }

        public bool IsEnabled { get; private set; }
    }
}