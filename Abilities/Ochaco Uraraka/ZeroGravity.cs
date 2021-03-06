﻿using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ochaco_Uraraka
{
    public class ZeroGravity : Ability
    {
        public override string Name { get; } = "Zero Gravity";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int FlyingDuration = 4;
        public ZeroGravity(Game game) : base(game)
        {
            OnAwake += () =>
            {
                ParentCharacter.CanAttackAllies = true;
                ParentCharacter.BasicAttack = character =>
                {
                    if (character.IsEnemyFor(Owner)) ParentCharacter.DefaultBasicAttack(character);
                    AddEffect(character);

                    ParentCharacter.HasUsedBasicAttackInPhaseBefore = true;
                };

            };
        }

        public void AddEffect(Character character) => character.Effects.Add(new Flying(Game, FlyingDuration, character, "Zero Gravity"));
        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} może atakować sojuszników, ale nie wyrządza im szkód.
Postacie zaatakowane przez nią potrafią latać przez {FlyingDuration} fazy.";
    }
}
