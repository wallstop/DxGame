using System;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public interface ICache<K, V>
    {
        bool GetIfPresent(K key, out V value);

        V Get(K key, Func<K, V> valueLoader);

        void Put(K key, V value);

        void Invalidate(K key);

        void InvalidateAll();

        long Count { get; }
    }
}
