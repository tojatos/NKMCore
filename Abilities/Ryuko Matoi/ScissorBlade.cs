using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ryuko_Matoi
{
    public class ScissorBlade : Ability
    {
        public override string Name { get; } = "Scissor Blade";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int PhysicalDefenseDecrease = 5;
        private const int Duration = 2;
        public ScissorBlade(Game game) : base(game)
        {
            OnAwake += () => ParentCharacter.BeforeBasicAttack += (character, damage) =>
                character.Effects.Add(new StatModifier(Game, Duration, -PhysicalDefenseDecrease, character,StatType.PhysicalDefense, Name));
        }

        public override string GetDescription() =>
$@"Podstawowe ataki {ParentCharacter.Name} zmniejszają obronę fizyczną przeciwników o {PhysicalDefenseDecrease} na {Duration} fazy.
Efekt ten nakłada się przed atakiem i może się kumulować.

Czas odnowienia: {Cooldown}";
    }
}
