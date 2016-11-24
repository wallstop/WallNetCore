using System;

namespace WallNetCore.Cache.Advanced
{
    public class CacheBuilder<K, V>
    {
        public long? ExpireAfterAccessMilliseconds { get; private set; }

        public long? ExpireAfterWriteMilliseconds { get; private set; }

        public long? MaxElements { get; private set; }

        public Action<RemovalNotification<K, V>> RemovalListener { get; private set; }

        private CacheBuilder() {}

        public ICache<K, V> Build()
        {
            return new LocalCache<K, V>(this);
        }

        public ILoadingCache<K, V> Build(Func<K, V> valueLoader)
        {
            return new LocalLoadingCache<K, V>(this, valueLoader);
        }

        public ILoadingCache<K, V> Build(Func<V> valueLoader)
        {
            return new LocalLoadingCache<K, V>(this, valueLoader);
        }

        public static CacheBuilder<K, V> NewBuilder()
        {
            return new CacheBuilder<K, V>();
        }

        public CacheBuilder<K, V> WithExpireAfterAccess(TimeSpan duration)
        {
            long durationMilliseconds = (long) Math.Round(duration.TotalMilliseconds);
            Validate.Validate.Hard.IsNotNegative(durationMilliseconds,
                () => $"Cannot expire after access of a duration of {durationMilliseconds}ms");
            ExpireAfterAccessMilliseconds = durationMilliseconds;
            return this;
        }

        public CacheBuilder<K, V> WithExpireAfterWrite(TimeSpan duration)
        {
            long durationMilliseconds = (long) Math.Round(duration.TotalMilliseconds);
            Validate.Validate.Hard.IsNotNegative(durationMilliseconds,
                () => $"Cannot expire after write of a duration of {durationMilliseconds}ms");
            ExpireAfterWriteMilliseconds = durationMilliseconds;
            return this;
        }

        public CacheBuilder<K, V> WithMaxElements(int maxElements)
        {
            Validate.Validate.Hard.IsPositive(maxElements,
                () => $"Cannot cap the max elements of a cache to be {maxElements}");
            MaxElements = maxElements;
            return this;
        }

        public CacheBuilder<K, V> WithRemovalListener(Action<RemovalNotification<K, V>> removalListener)
        {
            Validate.Validate.Hard.IsNotNull(removalListener, "Cannot register a null removalListener");
            RemovalListener = removalListener;
            return this;
        }
    }
}