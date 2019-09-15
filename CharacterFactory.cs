using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore
{
    public static class CharacterFactory
    {
        public static Character Create(Game game, string name) =>
            CreateCharacterFromDatabase(game, name, NKMID.GetNext("Character"));
        public static Character Create(Game game, string name, int id) =>
            CreateCharacterFromDatabase(game, name, id);

        private static Character CreateCharacterFromDatabase(Game game, string name, int id)
        {
            Dictionary<string, string> characterData = NKMData.GetCharacterData(name);
            Character.Properties properties = GetCharacterDbProperties(characterData);

            properties.Name = name;
            properties.Id = id;
            properties.Abilities = AbilityFactory.CreateAndInitiateAbilitiesFromDatabase(name, game);
            properties.Game = game;

            var createdCharacter = new Character(properties);

            game?.InvokeAfterCharacterCreation(createdCharacter);

            return createdCharacter;
        }

        private static Character.Properties GetCharacterDbProperties(Dictionary<string, string> characterData)
        {
            return new Character.Properties
            {
                AttackPoints =
                    new Stat(StatType.AttackPoints, int.Parse(characterData["AttackPoints"])),
                HealthPoints =
                    new Stat(StatType.HealthPoints, int.Parse(characterData["HealthPoints"])),
                BasicAttackRange = new Stat(StatType.BasicAttackRange,
                    int.Parse(characterData["BasicAttackRange"])),
                Speed = new Stat(StatType.Speed, int.Parse(characterData["Speed"])),
                PhysicalDefense = new Stat(StatType.PhysicalDefense,
                    int.Parse(characterData["PhysicalDefense"])),
                MagicalDefense = new Stat(StatType.MagicalDefense,
                    int.Parse(characterData["MagicalDefense"])),
                Shield = new Stat(StatType.Shield, 0),

                Type = characterData["FightType"].ToFightType(),
            };
        }
    }
}