﻿using System;

namespace DXGame.Core.Utils.Cache.Advanced
{
    public class CacheBuilder<K, V>
    {
        private const int UNSET_INT = -1;

        private const int DEFAULT_EXPIRATION_TICKS = 0;

        private long expireAfterAccessTicks_ = UNSET_INT;
        private long expireAfterWriteTicks_ = UNSET_INT;

        public Action<RemovalNotification<K, V>> RemovalListener { get; private set; }

        public long ExpireAfterAccessTicks
            => expireAfterAccessTicks_ == UNSET_INT ? DEFAULT_EXPIRATION_TICKS : expireAfterAccessTicks_;

        public long ExpireAfterWriteTicks
            => expireAfterWriteTicks_ == UNSET_INT ? DEFAULT_EXPIRATION_TICKS : expireAfterWriteTicks_;

        private CacheBuilder() {}

        public static CacheBuilder<K, V> NewBuilder()
        {
            return new CacheBuilder<K, V>();
        }

        public CacheBuilder<K, V> WithExpireAfterWrite(TimeSpan duration)
        {
            long durationTicks = duration.Ticks;
            Validate.IsTrue(durationTicks >= 0, "Cannot expire after write for a negative duration");
            expireAfterWriteTicks_ = durationTicks;
            return this;
        }

        public CacheBuilder<K, V> WithExpireAfterAccess(TimeSpan duration)
        {
            long durationTicks = duration.Ticks;
            Validate.IsTrue(durationTicks >= 0, "Cannot expire after access for a negative duration");
            expireAfterAccessTicks_ = durationTicks;
            return this;
        }

        public CacheBuilder<K, V> WithRemovalListener(Action<RemovalNotification<K, V>> removalListener)
        {
            Validate.IsNotNull(removalListener, "Cannot register a null removalListener");
            RemovalListener = removalListener;
            return this;
        }

        public ICache<K, V> Build()
        {
            return new LocalCache<K, V>(this);
        }

        public ILoadingCache<K, V> Build(Func<V> valueLoader)
        {
            return new LocalLoadingCache<K, V>(this, valueLoader);
        }

        private static void DoNothingRemovalListener(RemovalNotification<K, V> removalNotification) {}
    }
}
