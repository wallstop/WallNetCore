using System;

namespace WallNetCore.Cache.Advanced
{
    /**
        <summary>
            Extremely simple / fast Cache Key for use with Single-Element caches
        </summary>
    */

    [Serializable]
    public sealed class FastCacheKey : IEquatable<FastCacheKey>
    {
        public static readonly FastCacheKey Instance = new FastCacheKey();

        private FastCacheKey() {}

        public bool Equals(FastCacheKey other) => !ReferenceEquals(other, null);

        public override bool Equals(object other) => Equals(other as FastCacheKey);

        // Fair dice roll
        public override int GetHashCode() => 9;

        public override string ToString() => nameof(FastCacheKey);
    }
}