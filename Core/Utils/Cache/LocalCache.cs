using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.Utils.Cache
{
    public class LocalCache<K, V>
    {
        private const int MAXIMUM_CAPACITY = 1 << 30;
        private const int MAX_SEGMENTS = 1 << 16;
        private const int CONTAINS_VALUE_RETRIES = 1 << 3;

        private const int DRAIN_THRESHOLD = 0x3F;
        private const int DRAIN_MAX = 16;

        private readonly int segmentMask_;
        private readonly int segmentShift_;

        // TODO: SEGMENTS

        private readonly int concurrencyLevel_;
        private readonly long maximumWeight_;
        private readonly IWeigher<K, V> weigher_;

        private readonly long expireAfterAccessTicks_;
        private readonly long expireAfterWriteTicks_;
        private readonly long refreshTicks_;

        private readonly IProducerConsumerCollection<RemovalNotification<K, V>> removalNotificationQueue_;

        private readonly IRemovalListener<K, V> removalListener_;
        private readonly Stopwatch ticker_;

        private CacheLoader<K, V> defaultLoader_;



        public bool ExpiresAfterWrite => expireAfterWriteTicks_ > 0;
        public bool ExpiresAfterAccess => expireAfterAccessTicks_ > 0;
        public bool Refreshes => refreshTicks_ > 0;
        public bool RecordsWrite => (ExpiresAfterWrite || Refreshes);
        public bool RecordsAccess => ExpiresAfterAccess;
        public bool RecordsTime => (RecordsWrite || RecordsAccess);

        public LocalCache(CacheBuilder<K, V> builder, CacheLoader<K, V> loader = null)
        {
            Validate.IsNotNull(builder);

            concurrencyLevel_ = Math.Min(builder.ConcurrencyLevel, MAX_SEGMENTS);
            maximumWeight_ = builder.MaximumWeight;
            weigher_ = builder.Weigher;
            expireAfterWriteTicks_ = builder.ExpireAfterWriteTicks;
            expireAfterAccessTicks_ = builder.ExpireAfterAccessTicks;
            refreshTicks_ = builder.RefreshTicks;
            removalListener_ = builder.RemovalListener;
            if(ReferenceEquals(removalListener_, NullRemovalListener<K, V>.Instance))
            {
                removalNotificationQueue_ = DiscardingQueue<RemovalNotification<K, V>>.Instance;
            }
            else
            {
                removalNotificationQueue_ = new ConcurrentQueue<RemovalNotification<K, V>>();
            }
            //ticker_ = new

            // TODO

        } 
    }

    public class LocalLoadingCache<K, V> : LocalManualCache<K, V>, ILoadingCache<K, V>
    {
        public LocalLoadingCache(CacheBuilder<K, V> cacheBuilder, CacheLoader<K, V> cacheLoader)
            : base(new LocalCache<K, V>(cacheBuilder, cacheLoader))
        {
        } 

        public V Get(K key)
        {
            throw new NotImplementedException();
        }

        public void Refresh(K key)
        {
            throw new NotImplementedException();
        }
    }

    public class LocalManualCache<K, V> : ICache<K, V>
    {
        private readonly LocalCache<K, V> localCache_; 

        public LocalManualCache(CacheBuilder<K, V> builder)
            : this(new LocalCache<K, V>(builder))
        {
        }

        protected LocalManualCache(LocalCache<K, V> localCache)
        {
            localCache_ = localCache;
        }

        public V GetIfPresent(K key)
        {
            throw new NotImplementedException();
        }

        public V Get(K key, Func<V> valueLoader)
        {
            throw new NotImplementedException();
        }

        public void Put(K key, V value)
        {
            throw new NotImplementedException();
        }

        public void Invalidate(K key)
        {
            throw new NotImplementedException();
        }

        public void InvalidateAll(IEnumerable<K> keys)
        {
            throw new NotImplementedException();
        }

        public void InvalidateAll()
        {
            throw new NotImplementedException();
        }

        public long Size { get; }
        public void CleanUp()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class DiscardingQueue<T> : IProducerConsumerCollection<T>
    {
        private static readonly Lazy<ReadOnlyCollection<T>> EMPTY_LIST_FOR_ENUMERATION = new Lazy<ReadOnlyCollection<T>>(() => new ReadOnlyCollection<T>(new List<T>(0)));

        private ReadOnlyCollection<T> EmptyList => EMPTY_LIST_FOR_ENUMERATION.Value;

        private static readonly Lazy<DiscardingQueue<T>> INSTANCE = new Lazy<DiscardingQueue<T>>(() => new DiscardingQueue<T>());

        public static DiscardingQueue<T> Instance => INSTANCE.Value;

        private DiscardingQueue()
        {
        } 

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EmptyList.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
        }

        public int Count => 0;
        public object SyncRoot => EmptyList;
        public bool IsSynchronized => true;
        public void CopyTo(T[] array, int index)
        {
        }

        public bool TryAdd(T item)
        {
            return true;
        }

        public bool TryTake(out T item)
        {
            item = default(T);
            return true;
        }

        public T[] ToArray()
        {
            return EmptyList.ToArray();
        }
    }
}
