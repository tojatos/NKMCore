using System;
using System.Collections.Generic;

namespace NKMCore.Extensions
{
    public static class SystemGeneric
    {
        /// <summary>
        /// Adds element to the list and returns it
        /// </summary>
        public static List<T> AddOne<T>(this List<T> list, T element)
        {
            list.Add(element);
            return list;
        }

        /// <summary>
        /// Returns random value from the list
        /// </summary>
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0) return default;
            var r = new Random();
            return list[r.Next(list.Count)];
        }

        /// <summary>
        /// Returns random element from the list using NKMRandom and NKMID
        /// </summary>
        public static T GetNKMRandom<T>(this List<T> list, NKMRandom random)
        {
            if (list.Count == 0) return default;
            return list[random.Get("System Generic Random" + NKMID.GetNext("System Generic Random"), 0, list.Count)];
        }

        /// <summary>
        /// Returns second last element from the list
        /// </summary>
        public static T SecondLast<T>(this List<T> list)
        {
            if (list.Count < 2) throw new ArgumentException("Sequence does not contain at least two elements.");
            return list[list.Count - 2];
        }

        /// <summary>
        /// Returns value from specific enum type that matches given string
        /// </summary>
        public static T ToEnum<T>(this string value) =>
            (T) Enum.Parse(typeof(T), value, true);

        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default) =>
            dict.TryGetValue(key, out TV value) ? value : defaultValue;
    }
}
