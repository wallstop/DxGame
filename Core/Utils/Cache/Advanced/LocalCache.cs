using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXGame.Core.Utils.Cache.Advanced
{
    public class LocalCache<K, V> : ICache<K, V>
    {
        private const int CONCURRENCY_LEVEL = 4;

        public long ExpireAfterAccessTicks { get; }
        public long ExpireAfterWriteTicks { get; }

        public bool ExpiresAfterAccess => ExpireAfterAccessTicks > 0;
        public bool ExpiresAfterWrite => ExpireAfterWriteTicks > 0;

        private readonly IProducerConsumerCollection<RemovalNotification<K, V>> removalNotificationQueue_;
        private readonly ConcurrentDictionary<K, StampedAndLockedValue<V>> cache_ = new ConcurrentDictionary<K, StampedAndLockedValue<V>>();

        private readonly IProducerConsumerCollection<StampedAccessEntry<K, V>> writeQueue_;
        private readonly IProducerConsumerCollection<StampedAccessEntry<K, V>> accessQueue_;  

        private readonly Action<RemovalNotification<K, V>> removalListener_;

        private readonly Stopwatch ticker_;
        private readonly ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private long NowTick => ticker_.ElapsedTicks;

        private long NewAccessExpiryTimeTick => (ExpiresAfterAccess ? ExpireAfterAccessTicks + NowTick : -1);
        private long NewWriteExpiryTimeTick => (ExpiresAfterWrite ? ExpireAfterWriteTicks + NowTick : -1);

        private int readCount_ = 0;

        public LocalCache(CacheBuilder<K, V> cacheBuilder)
        {
            Validate.IsNotNull(cacheBuilder, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(cacheBuilder)));
            ExpireAfterAccessTicks = cacheBuilder.ExpireAfterAccessTicks;
            ExpireAfterWriteTicks = cacheBuilder.ExpireAfterWriteTicks;
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
            ticker_ = Stopwatch.StartNew();
        }

        private void PreWriteCleanup(long now)
        {
            // TODO
        }

        private void PostWriteCleanup()
        {
            // TODO
        }

        private void PreReadCleanup(long now)
        {
            // TODO
        }

        private void PostReadCleanup()
        {
            // TODO
        }

        private void RunUnlockedCleanup()
        {
            ProcessPendingNotifications();
        }

        private void RunLockedCleanup(long nowTick)
        {
            if(lock_.TryEnterWriteLock(0))
            {
                try
                {
                    ExpireEntries(nowTick);
                    Interlocked.Exchange(ref readCount_, 0);
                }
                finally
                {
                    lock_.ExitWriteLock();
                }
            }
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
                if(lockedValue.Lock.TryEnterWriteLock(0))
                {
                    try
                    {
                        V expectedValue = stampedEntry.Value;
                        if(!Objects.Equals(expectedValue, lockedValue.Value))
                        {
                            return true;
                        }
                        StampedAndLockedValue<V> removedValue;
                        bool success = cache_.TryRemove(key, out removedValue);
                        Assert.IsTrue(success);
                    }
                    finally
                    {
                        lockedValue.Lock.ExitWriteLock();
                    }
                }
            }
            return true;
        }

        private void ProcessPendingNotifications()
        {
            RemovalNotification<K, V> notification;
            while(removalNotificationQueue_.TryTake(out notification))
            {
                // TODO: Exception handling
                removalListener_.Invoke(notification);
            }
        }

        private void EnqueueNotification(K key, V value, RemovalCause removalCause)
        {
            RemovalNotification<K, V> notification = new RemovalNotification<K, V>(key, value, removalCause);
            removalNotificationQueue_.TryAdd(notification);
        }

        public Optional<V> GetIfPresent(K key)
        {
            StampedAndLockedValue<V> lockedValue;
            bool exists = cache_.TryGetValue(key, out lockedValue);
            if(!exists)
            {
                return Optional<V>.Empty;
            }
            
            // TODO: Refactor
            if(ExpiresAfterAccess)
            {
                using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Write))
                {
                    lockedValue.AccessExpiry = NewAccessExpiryTimeTick;
                    return lockedValue.Value;
                }
            }

            return lockedValue.Value;
        }

        public V Get(K key, Func<V> valueLoader)
        {
            StampedAndLockedValue<V> lockedValue = cache_.GetOrAdd(key, keyArg => NewStampedAndLockedValue(valueLoader));
            if(ExpiresAfterAccess)
            {
                using(new CriticalRegion(lockedValue.Lock, CriticalRegion.LockType.Write))
                {
                    lockedValue.AccessExpiry = NewAccessExpiryTimeTick;
                    return lockedValue.Value;
                }
            }
            return lockedValue.Value;
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

        public void Put(K key, V value)
        {
            StampedAndLockedValue<V> lockedValue = NewStampedAndLockedValue(value);
            cache_.AddOrUpdate(key, lockedValue, (keyArg, existingValue) =>
            {
                using(new CriticalRegion(existingValue.Lock, CriticalRegion.LockType.Write))
                {
                    existingValue.AccessExpiry = NewAccessExpiryTimeTick;
                    existingValue.WriteExpiry = NewWriteExpiryTimeTick;
                    existingValue.Value = value;
                    return existingValue;
                }
            });
        }

        public void Invalidate(K key)
        {
            // TODO
            StampedAndLockedValue<V> lockedValue;
            cache_.TryRemove(key, out lockedValue);
            throw new NotImplementedException();
        }

        public void InvalidateAll()
        {
            // TODO
            cache_.Clear();
        }

        public long Size => cache_.Count;

        public void CleanUp()
        {
            long now = NowTick;
            RunLockedCleanup(now);
            RunUnlockedCleanup();
        }
    }

    internal class StampedAndLockedValue<V>
    {
        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        public V Value { get; set; }
        public long AccessExpiry { get; set; }
        public long WriteExpiry { get; set; }

        public bool IsExpired(long nowTick)
        {
            return AccessExpiry > nowTick || WriteExpiry > nowTick;
        }

        public StampedAndLockedValue(V value, long accessExpiration, long writeExpiration)
        {
            Value = value;
            AccessExpiry = accessExpiration;
            WriteExpiry = writeExpiration;
        } 
    }

    internal class StampedAccessEntry<K, V>
    {
        public K Key { get; }
        public V Value { get; }
        public long TickStamp { get; }

        public StampedAccessEntry(K key, V value, long tickStamp)
        {
            Key = key;
            Value = value;
            TickStamp = tickStamp;
        } 
    }
}
