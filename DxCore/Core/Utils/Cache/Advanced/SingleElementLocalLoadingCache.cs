using System;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public class SingleElementLocalLoadingCache<V> : SingleElementLocalCache<V>, ILoadingCache<FastCacheKey, V>
    {
        private Func<V> ValueLoader { get; }

        public SingleElementLocalLoadingCache(CacheBuilder<FastCacheKey, V> cacheBuilder, Func<V> valueLoader)
            : base(cacheBuilder)
        {
            Validate.Validate.Hard.IsNotNull(valueLoader,
                () => this.GetFormattedNullOrDefaultMessage(nameof(valueLoader)));
            ValueLoader = valueLoader;
        }

        public V Get(FastCacheKey key)
        {
            return Get();
        }

        public V Get()
        {
            return Get(ValueLoader);
        }
    }
}
