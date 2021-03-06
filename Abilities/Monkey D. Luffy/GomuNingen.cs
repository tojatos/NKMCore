﻿using NKMCore.Templates;

namespace NKMCore.Abilities.Monkey_D._Luffy
{
    public class GomuNingen : Ability, IEnableable
    {
        public override string Name { get; } = "Gomu Ningen";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private int _timesAttacked;
        public GomuNingen(Game game) : base(game)
        {
            OnAwake += () => ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
                if (character.Type != FightType.Ranged) return;
                if(IsEnabled) damage.Value = 0;
                _timesAttacked++;
            };
        }

        public override string GetDescription() => $"{ParentCharacter.Name} blokuje co drugi podstawowy atak zasięgowy.";
        public bool IsEnabled => _timesAttacked % 2 == 0;
    }
}
