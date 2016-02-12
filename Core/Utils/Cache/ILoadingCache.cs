namespace DXGame.Core.Utils.Cache
{
    public interface ILoadingCache<in K, V> : ICache<K, V>
    {
        V Get(K key);
        void Refresh(K key);
    }
}
