﻿using System.Collections.Generic;
using NKMCore.Templates;

namespace NKMCore.Abilities.Hecate
{
    public class ItadakiNoKura : Ability
    {
        public override string Name { get; } = "Itadaki no Kura";
        protected override int Cooldown { get; } = 0;
        public override AbilityType Type { get; } = AbilityType.Passive;

        private const int HealthPercent = 40;
        public List<Character> CollectedEnergyCharacters { get; } = new List<Character>();
        public event Delegates.CharacterCharacter AfterCollectingEnergy;
        public int CollectedEnergy { get {
            float energy = 0;
            CollectedEnergyCharacters.ForEach(c => energy += c.HealthPoints.BaseValue * ((float) HealthPercent /100));
            return (int) energy;
        } }
        public ItadakiNoKura(Game game) : base(game)
        {
            OnAwake += () => ParentCharacter.AfterAttack += (character, damage) => TryCollectingEnergy(character);
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} gromadzi Energię Życiową każdym podstawowym atakiem lub z Astera.
Jeden ładunek Energii Życiowej przechowuje {HealthPercent}% maksymalnego HP celu.
Energia Życiowa tej samej postaci może zostać zgromadzona tylko raz.
Aktualna wartość zebranej energii: {CollectedEnergy}";

        public void TryCollectingEnergy(Character targetCharacter)
        {
            if (CollectedEnergyCharacters.Contains(targetCharacter)) return;

            CollectedEnergyCharacters.Add(targetCharacter);
            AfterCollectingEnergy?.Invoke(ParentCharacter, targetCharacter);
        }
    }
}
