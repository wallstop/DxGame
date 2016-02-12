using System;
using System.Threading.Tasks;

namespace DXGame.Core.Utils.Cache
{
    public abstract class CacheLoader<K, V>
    {
        public abstract V Load(K key);

        public virtual Task<V> Reload(K key, V oldValue)
        {
            Validate.IsNotNull(key);
            Validate.IsNotNull(oldValue);
            return new Task<V>(() => Load(key));
        }

        public static CacheLoader<K, V> From(Func<K, V> loadFunction)
        {
            Validate.IsNotNull(loadFunction);
            return new FunctionToCacheLoader<K, V>(loadFunction);
        }

        public static CacheLoader<K, V> AsyncReloading(CacheLoader<K, V> cacheLoader)
        {
            Validate.IsNotNull(cacheLoader);
            return new AsyncReloadingCacheLoader<K, V>(cacheLoader);
        }
    }

    internal sealed class FunctionToCacheLoader<K, V> : CacheLoader<K, V>
    {
        private readonly Func<K, V> loadFunction_;

        public FunctionToCacheLoader(Func<K, V> loadFunction)
        {
            loadFunction_ = loadFunction;
        }

        public override V Load(K key)
        {
            return loadFunction_.Invoke(key);
        }
    }

    internal sealed class AsyncReloadingCacheLoader<K, V> : CacheLoader<K, V>
    {
        private readonly CacheLoader<K, V> existingCacheLoader_;

        public AsyncReloadingCacheLoader(CacheLoader<K, V> existingCacheLoader)
        {
            existingCacheLoader_ = existingCacheLoader;
        }

        public override V Load(K key)
        {
            return existingCacheLoader_.Load(key);
        }

        public override Task<V> Reload(K key, V oldValue)
        {
            Task<V> reloadTask = new Task<V>(() => existingCacheLoader_.Reload(key, oldValue).Result);
            return reloadTask;
        }
    }

    internal sealed class ProducerCacheLoader<K, V> : CacheLoader<K, V>
    {
        private readonly Func<V> producer_;

        public ProducerCacheLoader(Func<V> producer)
        {
            producer_ = producer;
        }

        public override V Load(K key)
        {
            Validate.IsNotNull(key);
            return producer_.Invoke();
        }
    }
}
