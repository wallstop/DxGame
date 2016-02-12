using System;

namespace DXGame.Core.Utils.Cache
{
    public class NullRemovalListener<K, V> : IRemovalListener<K, V>
    {
        private static readonly Lazy<NullRemovalListener<K, V>> INSTANCE =
            new Lazy<NullRemovalListener<K, V>>(() => new NullRemovalListener<K, V>());

        public static NullRemovalListener<K, V> Instance => INSTANCE.Value;

        private NullRemovalListener() {}

        public void OnRemoval(RemovalNotification<K, V> removalNotification)
        {
            // No-op
        }
    }
}
