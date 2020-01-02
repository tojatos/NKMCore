using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Llenn
{
    public class PChan : Ability
    {
        public override string Name { get; } = "P-Chan";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int SpeedIncrease = 2;
        public PChan(Game game) : base(game)
        {
            OnAwake += () =>
            {
                Game.Characters.ForEach(AddFunctionality);
                Game.AfterCharacterCreation += AddFunctionality;
            };
        }

        private void AddFunctionality(Character c)
        {
            if(c.IsEnemyFor(Owner)) return;
            c.OnDeath += () => ParentCharacter.Speed.Value = ParentCharacter.Speed.RealValue + SpeedIncrease;
        }

        public override string GetDescription() =>
$@"Każda śmierć przyjaznej postaci zwiększa wolę do działania {ParentCharacter.Name},
co daje jej {SpeedIncrease} szybkości na stałe.";
    }
}
