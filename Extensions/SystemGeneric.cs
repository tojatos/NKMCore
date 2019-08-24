using System;
using System.Collections.Generic;

namespace NKMCore.Extensions
{
	public static class SystemGeneric
	{
		public static List<T> AddOne<T>(this List<T> list, T element)
		{
			list.Add(element);
			return list;
		}

		public static T GetRandom<T>(this List<T> list)
		{
			if (list.Count == 0) return default(T);
			var r = new Random();
			return list[r.Next(list.Count)];
		}
		public static T GetNKMRandom<T>(this List<T> list, NKMRandom random)
		{
			if (list.Count == 0) return default(T);
			return list[random.Get("System Generic Random" + NKMID.GetNext("System Generic Random"), 0, list.Count)];
		}
		public static T SecondLast<T>(this List<T> list)
		{
			if (list.Count < 2) throw new Exception("Sequence does not contain at least two elements.");
			return list[list.Count - 2];
		}
		public static T ToEnum<T>(this string value)
		{
			return (T) Enum.Parse(typeof(T), value, true);
		}

        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default)
        {
            return dict.TryGetValue(key, out TV value) ? value : defaultValue;
        }
	}
}
