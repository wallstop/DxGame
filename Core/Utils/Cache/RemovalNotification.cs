using System.Collections.Generic;
using System.ComponentModel;

namespace DXGame.Core.Utils.Cache
{
    public enum RemovalCause
    {
        EXPLICIT,
        REPLACED,
        COLLECTED,
        EXPIRED,
        SIZE
    }

    public class RemovalNotification<K, V>
    {
        public RemovalNotification(K key, V value, RemovalCause removalCause)
        {
            Key = key;
            Value = value;
            Cause = removalCause;
        }

        /* TODO: Cache? */
        public KeyValuePair<K, V> KeyValuePair => new KeyValuePair<K, V>(Key, Value);
        public V Value { get; }
        public K Key { get; }
        public RemovalCause Cause { get; }

        public override int GetHashCode()
        {
            return Objects.HashCode(Key, Value, Cause);
        }

        public override string ToString()
        {
            return KeyValuePair.ToString();
        }

        public override bool Equals(object other)
        {
            RemovalNotification<K, V> removalNotification = other as RemovalNotification<K, V>;
            if(ReferenceEquals(removalNotification, null))
            {
                return false;
            }
            return Objects.Equals(Key, removalNotification.Key) && Objects.Equals(Value, removalNotification.Value);
        }
    }

    public static class RemovalCauseExtensions
    {
        public static bool Evicted(this RemovalCause removalCause)
        {
            switch(removalCause)
            {
                case RemovalCause.EXPLICIT:
                    return false;
                case RemovalCause.REPLACED:
                    return false;
                case RemovalCause.COLLECTED:
                    return true;
                case RemovalCause.EXPIRED:
                    return true;
                case RemovalCause.SIZE:
                    return true;
                default:
                    throw new InvalidEnumArgumentException($"Unknown eviction cause for {removalCause}");
            }
        }
    }
}