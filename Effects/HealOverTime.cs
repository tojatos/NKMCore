﻿using NKMCore.Templates;

namespace NKMCore.Effects
{
    public class HealOverTime : Effect
    {
        private readonly int _healPerTick;

        public HealOverTime(Game game, Character characterThatHeals, int healPerTick, int cooldown, Character parentCharacter, string name = null) : base(game, cooldown, parentCharacter, name)
        {
            Name = name ?? "Heal Over Time";
            _healPerTick = healPerTick;
            Type = EffectType.Positive;
            void TryToActivateEffect() => characterThatHeals.Heal(ParentCharacter, healPerTick);
            ParentCharacter.JustBeforeFirstAction += TryToActivateEffect;
            OnRemove += () => ParentCharacter.JustBeforeFirstAction -= TryToActivateEffect;
        }
        public override string GetDescription()
        {
            return "Leczy " + _healPerTick + " HP co fazę.\n" +
                         "Czas do zakończenia efektu: " + CurrentCooldown;
        }
    }
}
