using System;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public class SingleElementLocalCache<V> : LocalCache<FastCacheKey, V>
    {
        public SingleElementLocalCache(CacheBuilder<FastCacheKey, V> cacheBuilder) : base(cacheBuilder) {}

        public bool GetIfPresent(out V value)
        {
            return GetIfPresent(FastCacheKey.Instance, out value);
        }

        public V Get(Func<V> valueLoader)
        {
            return Get(FastCacheKey.Instance, valueLoader);
        }

        public void Put(V value)
        {
            Put(FastCacheKey.Instance, value);
        }

        public void Invalidate()
        {
            Invalidate(FastCacheKey.Instance);
        }
    }
}
