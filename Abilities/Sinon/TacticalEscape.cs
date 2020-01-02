using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sinon
{
    public class TacticalEscape : Ability, IClickable
    {
        public override string Name { get; } = "Tactical Escape";
        protected override int Cooldown { get; } = 4;
        public override AbilityType Type { get; } = AbilityType.Normal;

        private const int SpeedIncrease = 8;
        private const int Duration = 1;

        public TacticalEscape(Game game) : base(game){}
        public override string GetDescription() =>
$@"Zwiększa szybkość {ParentCharacter.Name} o {SpeedIncrease}.
Czas trwania: {Duration}    Czas odnowienia: {Cooldown}";

        public void Click()
        {
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.Effects.Add(new StatModifier(Game, Duration, SpeedIncrease, ParentCharacter, StatType.Speed, Name));
            Finish();

        }
    }
}
