using DXGame.Core.Utils.Cache.Advanced;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public interface ILoadingCache<in K, V> : ICache<K, V>
    {
        V Get(K key);
    }
}
