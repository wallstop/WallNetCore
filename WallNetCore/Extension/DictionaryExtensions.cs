using System;
using System.Collections.Generic;
using System.Linq;

namespace WallNetCore.Extension
{
    public static class DictionaryExtensions
    {
        public static V GetOrElse<K, V>(this Dictionary<K, V> dictionary, K key, Func<V> valueProducer)
        {
            V value;
            if(dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            return valueProducer.Invoke();
        }

        public static V GetOrElse<K, V>(this Dictionary<K, V> dictionary, K key, V value)
            => GetOrElse(dictionary, key, () => value);

        public static Dictionary<K, V> ToDictionary<K, V>(this Dictionary<K, V> dictionary)
            => new Dictionary<K, V>(dictionary);

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> prettyMuchADictionary)
        {
            return prettyMuchADictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}