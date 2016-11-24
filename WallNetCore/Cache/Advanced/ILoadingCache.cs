namespace WallNetCore.Cache.Advanced
{
    public interface ILoadingCache<K, V> : ICache<K, V>
    {
        V Get(K key);
    }
}