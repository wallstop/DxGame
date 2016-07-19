namespace DxCore.Core.Utils.Cache
{
    public enum RemovalCause
    {
        Explicit,
        Replaced,
        Expired,
        Evicted
    }

    public sealed class RemovalNotification<K, V>
    {
        public K Key { get; }
        public V Value { get; }
        public RemovalCause RemovalCause { get; }

        public RemovalNotification(K key, V value, RemovalCause removalCause)
        {
            Validate.Validate.Hard.IsNotNull(removalCause);
            Key = key;
            Value = value;
            RemovalCause = removalCause;
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Key, Value);
        }

        public override bool Equals(object other)
        {
            RemovalNotification<K, V> notification = other as RemovalNotification<K, V>;
            if(!ReferenceEquals(notification, null))
            {
                return Objects.Equals(Key, notification.Key) && Objects.Equals(Value, notification.Value);
            }
            return false;
        }
    }
}
