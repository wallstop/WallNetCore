using System;

namespace WallNetCore.Cache.Advanced
{
    public interface ICache<K, V>
    {
        long Count { get; }

        V Get(K key, Func<K, V> valueLoader);

        bool GetIfPresent(K key, out V value);

        void Invalidate(K key);

        void InvalidateAll();

        void Put(K key, V value);
    }
}