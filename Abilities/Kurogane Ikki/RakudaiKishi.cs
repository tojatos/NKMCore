using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Kurogane_Ikki
{
    public class RakudaiKishi : Ability
    {
        public override string Name { get; } = "Rakudai Kishi";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int HealAmount = 15;
        public RakudaiKishi(Game game) : base(game)
        {
            OnAwake += () =>
            {
                Game.Characters.ForEach(AddFunctionality);
                Game.AfterCharacterCreation += AddFunctionality;
            };
        }

        private void AddFunctionality(Character character)
        {
            if (character.IsEnemyFor(Owner))
            {
                character.OnDeath += () => ParentCharacter.Heal(ParentCharacter, HealAmount);
            }
        }

        public override string GetDescription() =>
            $"Przy każdej śmierci wrogiej postaci {ParentCharacter.Name} leczy się za {HealAmount} HP.";
    }
}
