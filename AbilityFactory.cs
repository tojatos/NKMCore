﻿using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Templates;

namespace NKMCore
{
    public static class AbilityFactory
    {
        public static List<Ability> CreateAndInitiateAbilitiesFromDatabase(string name, IGame game)
        {
            IEnumerable<string> abilityClassNames = NKMData.GetAbilityClassNames(name);
            List<Ability> abilities = SpawnAbilities(name, abilityClassNames, game);
            abilities = abilities.OrderBy(a => a.Type).ToList();
            abilities.ForEach(a => game?.InvokeAfterAbilityCreation(a));
            return abilities;
        }

        public static Ability CreateAndInit(Type type, IGame game)
        {
            var a = Instantiator.Create<Ability>(type, game);
            game?.InvokeAfterAbilityCreation(a);
            return a;
        }

        private static List<Ability> SpawnAbilities(string name, IEnumerable<string> abilityClassNames, IGame game)
        {
            string abilityNamespaceName = "Abilities." + name.Replace(' ', '_');
            List<Ability> abilities = Instantiator.Create<Ability>(abilityNamespaceName, abilityClassNames, game).ToList();
            return abilities;
        }
    }
}