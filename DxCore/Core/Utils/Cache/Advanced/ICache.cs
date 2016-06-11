using System;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public interface ICache<in K, V>
    {
        Optional<V> GetIfPresent(K key);

        V Get(K key, Func<V> valueLoader);

        void Put(K key, V value);

        void Invalidate(K key);

        void InvalidateAll();

        long Size { get; }

        void CleanUp();
    }
}
