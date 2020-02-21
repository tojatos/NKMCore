using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Templates;

namespace NKMCore
{
    public static class Instantiator
    {
        private static T Create<T>(string namespaceName, string className, IGame game) where T : class
        {
            string typeName = "NKMCore." + namespaceName + "." + className;
            Type type = Type.GetType(typeName);

            return Create<T>(type, game);
        }

        public static T Create<T>(Type type, IGame game) where T : class => Activator.CreateInstance(type, game) as T;

        public static IEnumerable<T> Create<T>(string namespaceName, IEnumerable<string> classNames, IGame game) where T : class
        {
            return classNames.Select(className => Create<T>(namespaceName, className, game)).ToList();
        }
    }
}