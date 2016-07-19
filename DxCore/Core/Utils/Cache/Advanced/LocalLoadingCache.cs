using System;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public class LocalLoadingCache<K, V> : LocalCache<K, V>, ILoadingCache<K, V>
    {
        private Func<K, V> ValueLoader { get; }

        public LocalLoadingCache(CacheBuilder<K, V> cacheBuilder, Func<V> valueLoader)
            : this(cacheBuilder, ReferenceEquals(valueLoader, null) ? null : (Func<K, V>) (key => valueLoader.Invoke())) {}

        public LocalLoadingCache(CacheBuilder<K, V> cacheBuilder, Func<K, V> valueLoader) : base(cacheBuilder)
        {
            Validate.Validate.Hard.IsNotNull(valueLoader,
                () => this.GetFormattedNullOrDefaultMessage(nameof(valueLoader)));
            ValueLoader = valueLoader;
        }

        public V Get(K key)
        {
            return Get(key, ValueLoader);
        }
    }
}
