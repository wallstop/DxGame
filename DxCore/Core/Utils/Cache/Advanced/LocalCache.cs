using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DXGame.Core.Utils.Cache.Advanced
{
    public class LocalCache<K, V> : ICache<K, V>
    {
        private const int DRAIN_THRESHOLD = 0x3F;
        private readonly IProducerConsumerCollection<StampedAccessEntry<K>> accessQueue_;

        private readonly ConcurrentDictionary<K, StampedAndLockedValue<V>> cache_ =
            new ConcurrentDictionary<K, StampedAndLockedValue<V>>();

        private readonly Action<RemovalNotification<K, V>> removalListener_;

        private readonly IProducerConsumerCollection<RemovalNotification<K, V>> removalNotificationQueue_;

        private readonly Stopwatch ticker_;

        private readonly IProducerConsumerCollection<StampedAccessEntry<K>> writeQueue_;

        private int readCount_;

        public long ExpireAfterAccessTicks { get; }
        public long ExpireAfterWriteTicks { get; }

        public bool ExpiresAfterAccess => ExpireAfterAccessTicks > 0;
        public bool ExpiresAfterWrite => ExpireAfterWriteTicks > 0;

        private long NewAccessExpiryTimeTick => AccessExpiryTimeTick(NowTick);
        private long NewWriteExpiryTimeTick => WriteExpiryTimeTick(NowTick);

        private long NowTick => ticker_.ElapsedTicks;

        private readonly ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public LocalCache(CacheBuilder<K, V> cacheBuilder)
        {
            Validate.IsNotNull(cacheBuilder, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(cacheBuilder)));
            ticker_ = Stopwatch.StartNew();
            /* Our input ticks are of the form "Timespan Ticks". We need to transform them to "Stopwatch" ticks */
            ExpireAfterAccessTicks = TransformTimeSpanTicksToStopwatchTicks(cacheBuilder.ExpireAfterAccessTicks);
            ExpireAfterWriteTicks = TransformTimeSpanTicksToStopwatchTicks(cacheBuilder.ExpireAfterWriteTicks);

            removalListener_ = cacheBuilder.RemovalListener;
            if(ReferenceEquals(cacheBuilder.RemovalListener, null))
            {
                removalNotificationQueue_ = DiscardingQueue<RemovalNotification<K, V>>.Instance;
            }
            else
            {
                removalNotificationQueue_ = new ConcurrentQueue<RemovalNotification<K, V>>();
            }

            if(ExpiresAfterAccess)
            {
                accessQueue_ = new ConcurrentQueue<StampedAccessEntry<K>>();
            }
            else
            {
                accessQueue_ = DiscardingQueue<StampedAccessEntry<K>>.Instance;
            }

            if(ExpiresAfterWrite)
            {
                writeQueue_ = new ConcurrentQueue<StampedAccessEntry<K>>();
            }
            else
            {
                writeQueue_ = DiscardingQueue<StampedAccessEntry<K>>.Instance;
            }
        }

        private static int TransformTimeSpanTicksToStopwatchTicks(long ticks)
        {
            return (int) Math.Round(1.0 * Stopwatch.Frequency / new TimeSpan(ticks).TotalSeconds);
        }

        public Optional<V> GetIfPresent(K key)
        {
            try
            {
                /* We do not need to grab the global lock - locking is only for sane modification of the map. GetIfPresent does no modification */
                StampedAndLockedValue<V> lockedValue;
                bool exists = cache_.TryGetValue(key, out lockedValue);
                if(!exists)
                {
                    return Optional<V>.Empty;
                }
                return UpdateOrExpireStampedAndLockedValueForRead(key, lockedValue);
            }
            finally
            {
                PostReadCleanup();
            }
        }

        private Optional<V> UpdateOrExpireStampedAndLockedValueForRead(K key, StampedAndLockedValue<V> lockedValue)
        {
            long nowTicks = NowTick;

            lockedValue.Lock.EnterUpgradeableReadLock();
            try
            {
                if(lockedValue.IsExpired(nowTicks))
                {
                    TryExpireEntries(nowTicks);
                    return Optional<V>.Empty;
                }

                /* Neat, new read, record it */
                RecordRead(key, nowTicks);
                V value = lockedValue.Value;
                if(ExpiresAfterAccess)
                {
                    lockedValue.Lock.EnterWriteLock();
                    try
                    {
                        lockedValue.AccessExpiry = AccessExpiryTimeTick(nowTicks);
                    }
                    finally
                    {
                        lockedValue.Lock.ExitWriteLock();
                    }
                }
                return value;
            }
            finally
            {
                lockedValue.Lock.ExitUpgradeableReadLock();
            }
        }

        public V Get(K key, Func<V> valueLoader)
        {
            try
            {
                /* TODO: See if we can do this by relying on the concurrentDictionary's atomic operations. Right now, this is *NOT* atomic */

                long currentTicks = NowTick;
                StampedAndLockedValue<V> lockedValue;
                using(new CriticalRegion(lock_, CriticalRegion.LockType.Read))
                {
                    lockedValue = cache_.AddOrUpdate(key, keyArg =>
                    {
                        StampedAndLockedValue<V> newLockedValue = NewStampedAndLockedValue(valueLoader);
                        /* Totally new - no need to lock it, we have TOTAL control */
                        RecordWrite(key, currentTicks);
                        return newLockedValue;
                    }, (keyArg, existingValue) =>
                    {
                        Optional<V> maybeExistingValue = UpdateOrExpireStampedAndLockedValueForRead(keyArg,
                            existingValue);
                        if(!maybeExistingValue.HasValue)
                        {
                            StampedAndLockedValue<V> newLockedValue = NewStampedAndLockedValue(valueLoader);
                            RecordWrite(key, currentTicks);
                            return newLockedValue;
                        }
                        return existingValue;
                    });
                }

                V foundValue;
                using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Read))
                {
                    foundValue = lockedValue.Value;
                    RecordRead(key, currentTicks);
                }
                /* Only lock if we care */
                if(ExpiresAfterAccess)
                {
                    using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Write))
                    {
                        lockedValue.AccessExpiry = AccessExpiryTimeTick(currentTicks);
                    }
                }
                return foundValue;
            }
            finally
            {
                PostReadCleanup();
            }
        }

        public void Put(K key, V value)
        {
            long nowTicks = NowTick;
            try
            {
                PreWriteCleanup(nowTicks);
                StampedAndLockedValue<V> lockedValue = NewStampedAndLockedValue(value);
                using(new CriticalRegion(lock_, CriticalRegion.LockType.Read))
                {
                    cache_.AddOrUpdate(key, keyArg => lockedValue, (keyArg, existingValue) =>
                    {
                        V foundValue;
                        using(new CriticalRegion(existingValue.Lock, CriticalRegion.LockType.Read))
                        {
                            foundValue = existingValue.Value;
                        }
                        /* Same value? No replacement notification, but we do want to update our timestamps */
                        if(Objects.Equals(value, foundValue))
                        {
                            RecordWrite(key, nowTicks);
                            if(ExpiresAfterWrite)
                            {
                                using(new CriticalRegion(existingValue.Lock, CriticalRegion.LockType.Write))
                                {
                                    existingValue.WriteExpiry = NewWriteExpiryTimeTick;
                                    return existingValue;
                                }
                            }
                        }
                        /* Otherwise, lock the value and update it */
                        using(new CriticalRegion(existingValue.Lock, CriticalRegion.LockType.Write))
                        {
                            EnqueueNotification(key, existingValue.Value, RemovalCause.Replaced);
                            existingValue.WriteExpiry = NewWriteExpiryTimeTick;
                            existingValue.Value = value;
                            return existingValue;
                        }
                    });
                }
            }
            finally
            {
                RecordWrite(key, nowTicks);
                PostWriteCleanup();
            }
        }

        public void Invalidate(K key)
        {
            try
            {
                long nowTicks = NowTick;
                PreWriteCleanup(nowTicks);
                StampedAndLockedValue<V> lockedValue;
                if(cache_.TryRemove(key, out lockedValue))
                {
                    using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Read))
                    {
                        EnqueueNotification(key, lockedValue.Value, RemovalCause.Explicit);
                    }
                }
            }
            finally
            {
                PostWriteCleanup();
            }
        }

        public void InvalidateAll()
        {
            try
            {
                lock_.EnterUpgradeableReadLock();
                try
                {
                    foreach(KeyValuePair<K, StampedAndLockedValue<V>> entry in cache_)
                    {
                        using(new CriticalRegion(entry.Value.Lock, CriticalRegion.LockType.Read))
                        {
                            EnqueueNotification(entry.Key, entry.Value.Value, RemovalCause.Explicit);
                        }
                    }
                    lock_.EnterWriteLock();
                    try
                    {
                        cache_.Clear();
                        writeQueue_.Clear();
                        accessQueue_.Clear();
                    }
                    finally
                    {
                        lock_.ExitWriteLock();
                    }
                    Interlocked.Exchange(ref readCount_, 0);
                }
                finally
                {
                    lock_.ExitUpgradeableReadLock();
                }
            }
            finally
            {
                PostWriteCleanup();
            }
        }

        public long Size => cache_.Count;

        public void CleanUp()
        {
            long now = NowTick;
            RunLockedCleanup(now);
            RunUnlockedCleanup();
        }

        private long AccessExpiryTimeTick(long nowTicks)
        {
            return ExpiresAfterAccess ? ExpireAfterAccessTicks + nowTicks : long.MaxValue;
        }

        private void EnqueueNotification(K key, V value, RemovalCause removalCause)
        {
            RemovalNotification<K, V> notification = new RemovalNotification<K, V>(key, value, removalCause);
            removalNotificationQueue_.TryAdd(notification);
        }

        private void ExpireEntries(long nowTick)
        {
            StampedAccessEntry<K> stampedEntry;
            while(writeQueue_.TryTake(out stampedEntry))
            {
                if(!ExpireEntry(stampedEntry, nowTick))
                {
                    break;
                }
            }
            while(accessQueue_.TryTake(out stampedEntry))
            {
                if(!ExpireEntry(stampedEntry, nowTick))
                {
                    break;
                }
            }
        }

        private bool ExpireEntry(StampedAccessEntry<K> stampedEntry, long nowTick)
        {
            if(!stampedEntry.IsExpired(nowTick))
            {
                return false;
            }

            K key = stampedEntry.Key;
            V value;
            StampedAndLockedValue<V> lockedValue;
            using(new CriticalRegion(lock_, CriticalRegion.LockType.Write))
            {
                if(cache_.TryGetValue(key, out lockedValue))
                {
                    using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Read))
                    {
                        if(stampedEntry.AccessExpiry != lockedValue.AccessExpiry ||
                           stampedEntry.WriteExpiry != lockedValue.WriteExpiry)
                        {
                            /* Value updated - we'll get a notification later! Return true so we continue to process */
                            return true;
                        }
                        value = lockedValue.Value;
                        /* Otherwise, remove the bad voy */
                        StampedAndLockedValue<V> removedValue;
                        bool success = cache_.TryRemove(key, out removedValue);
                        // TODO: Validate?
                    }
                }
                else
                {
                    /* Already expired, continue */
                    return true;
                }
            }
            EnqueueNotification(key, value, RemovalCause.Expired);
            return true;
        }

        private StampedAndLockedValue<V> NewStampedAndLockedValue(Func<V> valueLoader)
        {
            V value = valueLoader.Invoke();
            return NewStampedAndLockedValue(value);
        }

        private StampedAndLockedValue<V> NewStampedAndLockedValue(V value)
        {
            return new StampedAndLockedValue<V>(value, NewAccessExpiryTimeTick, NewWriteExpiryTimeTick);
        }

        private void PostReadCleanup()
        {
            if((Interlocked.Increment(ref readCount_) & DRAIN_THRESHOLD) == 0)
            {
                CleanUp();
            }
        }

        private void PostWriteCleanup()
        {
            RunUnlockedCleanup();
        }

        private void PreWriteCleanup(long now)
        {
            RunLockedCleanup(now);
        }

        private void ProcessPendingNotifications()
        {
            RemovalNotification<K, V> notification;
            while(removalNotificationQueue_.TryTake(out notification))
            {
                try
                {
                    removalListener_.Invoke(notification);
                }
                catch(Exception)
                {
                    // Ignore lol, go away
                }
            }
        }

        private void RecordRead(K key, long nowTicks)
        {
            StampedAccessEntry<K> stampedReadAccess = NewStampedAccessEntry(key, nowTicks);
            bool success = accessQueue_.TryAdd(stampedReadAccess);
            // Validate success?
        }

        private void RecordWrite(K key, long nowTicks)
        {
            StampedAccessEntry<K> stampedWriteAccess = NewStampedAccessEntry(key, nowTicks);
            bool success = writeQueue_.TryAdd(stampedWriteAccess);
            // Validate success?
        }

        private StampedAccessEntry<K> NewStampedAccessEntry(K key, long nowTicks)
        {
            return new StampedAccessEntry<K>(key, AccessExpiryTimeTick(nowTicks), WriteExpiryTimeTick(nowTicks));
        }

        private void RunLockedCleanup(long nowTick)
        {
            ExpireEntries(nowTick);
            Interlocked.Exchange(ref readCount_, 0);
        }

        private void RunUnlockedCleanup()
        {
            ProcessPendingNotifications();
        }

        private void TryExpireEntries(long nowTicks)
        {
            ExpireEntries(nowTicks);
        }

        private long WriteExpiryTimeTick(long nowTicks)
        {
            return ExpiresAfterWrite ? ExpireAfterWriteTicks + nowTicks : long.MaxValue;
        }
    }

    /**
        The "secret sauce" Key augmentation. Provides per-element locking for our own management
    */
    internal sealed class StampedAndLockedValue<V>
    {
        public long AccessExpiry { get; set; }
        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        public V Value { get; set; }
        public long WriteExpiry { get; set; }

        public StampedAndLockedValue(V value, long accessExpiration, long writeExpiration)
        {
            Value = value;
            AccessExpiry = accessExpiration;
            WriteExpiry = writeExpiration;
        }

        public bool IsExpired(long nowTick)
        {
            return (AccessExpiry < nowTick) || (WriteExpiry < nowTick);
        }
    }

    /**
        Records an Access snapshot for a given key. These entries are shoved onto a respective read / write queue. Once in awhile, the queues are flushed. We're able to gaurantee that an entry should be expired IFF:
        * The key exists in the cache
        * The access tickstamp is the same as this access entry
        * The write tickstamp is the same as this access entry

        Due to the following reasons: 
        * If a key->value mapping is overwritten, it will have a different write stamp.
        * If a key->value is accessed, it will have a different access stamp
        * If a key->value was removed, it will not exist in the cache

        Combined with the fact that we do not record StampedAccessEntries for operations that we don't care about (ie, configured, via a DiscardingQueue), 
        this allows us to 
    */

    internal sealed class StampedAccessEntry<K>
    {
        public K Key { get; }
        public long AccessExpiry { get; }
        public long WriteExpiry { get; }

        public StampedAccessEntry(K key, long accessExpireTick, long writeExpireTick)
        {
            Key = key;
            AccessExpiry = accessExpireTick;
            WriteExpiry = writeExpireTick;
        }

        public bool IsExpired(long nowTick)
        {
            return (AccessExpiry < nowTick) || (WriteExpiry < nowTick);
        }
    }
}