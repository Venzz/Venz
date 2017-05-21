using System.Collections.Generic;

namespace Venz.Extensions
{
    public static class DictionaryExtensions
    {
        public static V TryGet<T, V>(this IDictionary<T, V> source, T key) where V: class
        {
            V value = null;
            source.TryGetValue(key, out value);
            return value;
        }

        public static void AddOrReplace<T, V>(this IDictionary<T, V> source, T key, V value) where V: class
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }
    }
}
