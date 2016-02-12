using System;

namespace DXGame.Core.Utils.Cache
{
    public class SimpleWeigher<K, V> : IWeigher<K, V>
    {
        private static readonly Lazy<SimpleWeigher<K, V>> INSTANCE =
            new Lazy<SimpleWeigher<K, V>>(() => new SimpleWeigher<K, V>());

        public static SimpleWeigher<K, V> Instance => INSTANCE.Value;

        private SimpleWeigher() {}

        public int Weigh(K key, V value)
        {
            return 1;
        }
    }
}
