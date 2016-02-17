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
        private readonly IProducerConsumerCollection<StampedAccessEntry<K, V>> accessQueue_;

        private readonly ConcurrentDictionary<K, StampedAndLockedValue<V>> cache_ =
            new ConcurrentDictionary<K, StampedAndLockedValue<V>>();

        private readonly Action<RemovalNotification<K, V>> removalListener_;

        private readonly IProducerConsumerCollection<RemovalNotification<K, V>> removalNotificationQueue_;

        private readonly Stopwatch ticker_;

        private readonly IProducerConsumerCollection<StampedAccessEntry<K, V>> writeQueue_;

        private int readCount_;

        public long ExpireAfterAccessTicks { get; }
        public long ExpireAfterWriteTicks { get; }

        public bool ExpiresAfterAccess => ExpireAfterAccessTicks > 0;
        public bool ExpiresAfterWrite => ExpireAfterWriteTicks > 0;

        private long NewAccessExpiryTimeTick => AccessExpiryTimeTick(NowTick);
        private long NewWriteExpiryTimeTick => WriteExpiryTimeTick(NowTick);

        private long NowTick => ticker_.ElapsedTicks;

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
                accessQueue_ = new ConcurrentQueue<StampedAccessEntry<K, V>>();
            }
            else
            {
                accessQueue_ = DiscardingQueue<StampedAccessEntry<K, V>>.Instance;
            }

            if(ExpiresAfterWrite)
            {
                writeQueue_ = new ConcurrentQueue<StampedAccessEntry<K, V>>();
            }
            else
            {
                writeQueue_ = DiscardingQueue<StampedAccessEntry<K, V>>.Instance;
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
            if(lockedValue.IsExpired(nowTicks))
            {
                TryExpireEntries(nowTicks);
                return Optional<V>.Empty;
            }

            lockedValue.Lock.EnterUpgradeableReadLock();
            try
            {
                /* Neat, new read, record it */
                RecordRead(key, lockedValue.Value, nowTicks);
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
                StampedAndLockedValue<V> lockedValue;
                /* TODO: See if we can do this by relying on the concurrentDictionary's atomic operations. Right now, this is *NOT* atomic */

                /* First, see if a value exists. We need to check expiry here - it might be expired :( */
                if(cache_.TryGetValue(key, out lockedValue))
                {
                    Optional<V> maybeNonExpiredValue = UpdateOrExpireStampedAndLockedValueForRead(key, lockedValue);
                    if(maybeNonExpiredValue.HasValue)
                    {
                        return maybeNonExpiredValue.Value;
                    }
                }
                /* Someone might have beat us to the punch */

                long currentTicks = NowTick;
                lockedValue = cache_.AddOrUpdate(key, keyArg =>
                {
                    StampedAndLockedValue<V> newLockedValue = NewStampedAndLockedValue(valueLoader);
                    RecordWrite(key, newLockedValue.Value, currentTicks);
                    return newLockedValue;
                }, (keyArg, existingValue) => existingValue);

                V foundValue;
                using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Read))
                {
                    foundValue = lockedValue.Value;
                    RecordRead(key, foundValue, currentTicks);
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
            try
            {
                long nowTicks = NowTick;
                PreWriteCleanup(nowTicks);
                StampedAndLockedValue<V> lockedValue = NewStampedAndLockedValue(value);
                cache_.AddOrUpdate(key, keyArg =>
                {
                    RecordWrite(key, value, nowTicks);
                    return lockedValue;
                }, (keyArg, existingValue) =>
                {
                    V foundValue = existingValue.Value;
                    /* Same value? No replacement notification, but we do want to update our timestamps */
                    if(Objects.Equals(value, foundValue))
                    {
                        RecordWrite(key, value, nowTicks);
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
                        RecordWrite(key, value, nowTicks);
                        return existingValue;
                    }
                });
            }
            finally
            {
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
                foreach(KeyValuePair<K, StampedAndLockedValue<V>> entry in cache_)
                {
                    using(new CriticalRegion(entry.Value.Lock, CriticalRegion.LockType.Read))
                    {
                        EnqueueNotification(entry.Key, entry.Value.Value, RemovalCause.Explicit);
                    }
                }
                cache_.Clear();
                writeQueue_.Clear();
                accessQueue_.Clear();
                Interlocked.Exchange(ref readCount_, 0);
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
            StampedAccessEntry<K, V> stampedEntry;
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

        private bool ExpireEntry(StampedAccessEntry<K, V> stampedEntry, long nowTick)
        {
            K key = stampedEntry.Key;
            StampedAndLockedValue<V> lockedValue;
            if(cache_.TryGetValue(key, out lockedValue))
            {
                if(!lockedValue.IsExpired(nowTick))
                {
                    return false;
                }
                /* If we can't enter the write lock, it means that someone else owns it - and that they're fucking with it, which means don't expire */
                using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Read))
                {
                    V expectedValue = stampedEntry.Value;
                    if(!Objects.Equals(expectedValue, lockedValue.Value))
                    {
                        /* Our mapped value has changed - we don't care! We'll get a notification later */
                        return true;
                    }
                    StampedAndLockedValue<V> removedValue;
                    bool success = cache_.TryRemove(key, out removedValue);
                    // TODO: Validate? This should be true
                }
            }
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
                // TODO: Exception handling
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

        private void RecordRead(K key, V value, long nowTicks)
        {
            StampedAccessEntry<K, V> stampedReadAccess = new StampedAccessEntry<K, V>(key, value,
                AccessExpiryTimeTick(nowTicks));
            bool success = accessQueue_.TryAdd(stampedReadAccess);
            // Validate success?
        }

        private void RecordWrite(K key, V value, long nowTicks)
        {
            StampedAccessEntry<K, V> stampedWriteAccess = new StampedAccessEntry<K, V>(key, value,
                WriteExpiryTimeTick(nowTicks));
            bool success = writeQueue_.TryAdd(stampedWriteAccess);
            // Validate success?
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

    internal sealed class StampedAccessEntry<K, V>
    {
        public K Key { get; }
        public long TickStamp { get; }
        public V Value { get; }

        public StampedAccessEntry(K key, V value, long tickStamp)
        {
            Key = key;
            Value = value;
            TickStamp = tickStamp;
        }
    }
}