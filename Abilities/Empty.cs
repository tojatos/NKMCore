using NKMCore.Templates;

namespace NKMCore.Abilities
{
    public class Empty : Ability
    {
        public override string Name { get; } = "Pusta umiejętność";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; }

        public Empty(Game game, AbilityType type) : base(game)
        {
            Type = type;
        }
        public override string GetDescription() => "Twojego bohatera najwyraźniej nie stać na lepszą umiejętność.";
    }
}
