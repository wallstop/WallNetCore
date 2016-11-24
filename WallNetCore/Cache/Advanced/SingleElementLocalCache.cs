using System;

namespace WallNetCore.Cache.Advanced
{
    public class SingleElementLocalCache<V> : LocalCache<FastCacheKey, V>
    {
        public SingleElementLocalCache(CacheBuilder<FastCacheKey, V> cacheBuilder) : base(cacheBuilder) {}

        public V Get(Func<V> valueLoader)
        {
            return Get(FastCacheKey.Instance, instance => valueLoader.Invoke());
        }

        public bool GetIfPresent(out V value)
        {
            return GetIfPresent(FastCacheKey.Instance, out value);
        }

        public void Invalidate()
        {
            Invalidate(FastCacheKey.Instance);
        }

        public void Put(V value)
        {
            Put(FastCacheKey.Instance, value);
        }
    }
}