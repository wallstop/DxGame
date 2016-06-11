using System;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public class LocalLoadingCache<K, V> : LocalCache<K, V>, ILoadingCache<K, V>
    {
        private readonly Func<V> valueLoader_;

        public LocalLoadingCache(CacheBuilder<K, V> cacheBuilder, Func<V> valueLoader) : base(cacheBuilder)
        {
            Validate.IsNotNull(valueLoader, this.GetFormattedNullOrDefaultMessage(nameof(valueLoader)));
            valueLoader_ = valueLoader;
        }

        public V Get(K key)
        {
            return Get(key, valueLoader_);
        }
    }
}
