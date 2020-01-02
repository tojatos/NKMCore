using NKMCore.Templates;

namespace NKMCore.Abilities.Kirito
{
    public class Parry : Ability
    {
        public override string Name { get; } = "Parry";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int DodgeChancePercent = 25;

        public Parry(Game game) : base(game)
        {
            OnAwake += () => ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
                int r = Random.Get(Name, 1, 101);
                if (r <= DodgeChancePercent) damage.Value = 0;
            };
        }

        public override string GetDescription() =>
            $"{ParentCharacter.Name} ma {DodgeChancePercent}% szans na uniknięcie podstawowego ataku wrogiej postaci.";
    }
}
