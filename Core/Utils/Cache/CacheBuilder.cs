using System;

namespace DXGame.Core.Utils.Cache
{
    public sealed class CacheBuilder<K, V>
    {
        private static readonly int UNSET_INT = -1;

        private const int DEFAULT_INITIAL_CAPACITY = 16;
        private const int DEFAULT_CONCURRENCY_LEVEL = 4;
        private const int DEFAULT_EXPIRATION_TICKS = 0;
        private const int DEFAULT_REFRESH_TICKS = 0;

        private readonly bool strictParsing_ = true;
        private long maximumSize_ = UNSET_INT;
        private readonly long maximumWeight_ = UNSET_INT;

        public int InitialCapacity => DEFAULT_INITIAL_CAPACITY;
        public int ConcurrencyLevel => DEFAULT_CONCURRENCY_LEVEL;

        public long MaximumWeight
        {
            get
            {
                if(expireAfterWriteTicks_ == 0 || expireAfterAccessTicks_ == 0)
                {
                    return 0;
                }
                return ReferenceEquals(weigher_, null) ? maximumSize_ : maximumWeight_;
            }
        }

        private IWeigher<K, V> weigher_;

        public IWeigher<K, V> Weigher => ReferenceEquals(weigher_, null) ? SimpleWeigher<K, V>.Instance : weigher_;

        private long expireAfterWriteTicks_ = UNSET_INT;
        private long expireAfterAccessTicks_ = UNSET_INT;
        private long refreshTicks_ = UNSET_INT;

        public long ExpireAfterWriteTicks
            => expireAfterWriteTicks_ == UNSET_INT ? DEFAULT_EXPIRATION_TICKS : expireAfterWriteTicks_;

        public long ExpireAfterAccessTicks
            => expireAfterAccessTicks_ == UNSET_INT ? DEFAULT_EXPIRATION_TICKS : expireAfterAccessTicks_;

        public long RefreshTicks => refreshTicks_ == UNSET_INT ? DEFAULT_REFRESH_TICKS : refreshTicks_;

        private IRemovalListener<K, V> removalListener_;

        public IRemovalListener<K, V> RemovalListener
            => ReferenceEquals(removalListener_, null) ? NullRemovalListener<K, V>.Instance : removalListener_;

        private CacheBuilder() {}

        public static CacheBuilder<K, V> NewBuilder()
        {
            return new CacheBuilder<K, V>();
        }

        public CacheBuilder<K, V> WithMaximumSize(long size)
        {
            Validate.AreEqual(maximumSize_, UNSET_INT, $"Maximum Size was already set to {maximumSize_}");
            Validate.AreEqual(maximumWeight_, UNSET_INT, $"Maximum weight was already set to {maximumWeight_}");
            Validate.IsNull(weigher_, "Maximum size cannot be combined with a weigher");
            Validate.IsTrue(size >= 0, "Maximum size cannot be negative");
            maximumSize_ = size;
            return this;
        }

        public CacheBuilder<K, V> WithExpireAfterWrite(TimeSpan duration)
        {
            Validate.AreEqual(expireAfterWriteTicks_, UNSET_INT,
                $"ExpireAfterWrite was already set to {expireAfterWriteTicks_} ticks");
            long ticks = duration.Ticks;
            return WithExpireAfterWrite(ticks);
        }

        public CacheBuilder<K, V> WithExpireAfterWrite(long ticks)
        {
            Validate.IsTrue(ticks >= 0, "Cannot ExpireAfterWrite for a negative tick value");
            expireAfterWriteTicks_ = ticks;
            return this;
        }

        public CacheBuilder<K, V> WithExpireAfterAccess(TimeSpan duration)
        {
            Validate.AreEqual(expireAfterAccessTicks_, UNSET_INT,
                $"ExpireAfterAccess was already set to {expireAfterAccessTicks_}");
            long ticks = duration.Ticks;
            return WithExpireAfterAccess(ticks);
        }

        public CacheBuilder<K, V> WithExpireAfterAccess(long ticks)
        {
            Validate.IsTrue(ticks >= 0, $"Cannot ExpireAfterAccess for a negative tick value");
            expireAfterAccessTicks_ = ticks;
            return this;
        }

        public CacheBuilder<K, V> WithRefreshAfterWrite(TimeSpan duration)
        {
            Validate.AreEqual(refreshTicks_, UNSET_INT, $"RefreshAfterWrite was already set to {refreshTicks_}");
            long ticks = duration.Ticks;
            return WithRefreshAfterWrite(ticks);
        }

        public CacheBuilder<K, V> WithRefreshAfterWrite(long ticks)
        {
            Validate.IsTrue(ticks >= 0, "Cannot RefreshAfterWrite for a negative tick value");
            refreshTicks_ = ticks;
            return this;
        }

        public CacheBuilder<K, V> WithRemovalListener(IRemovalListener<K, V> removalListener)
        {
            Validate.IsNull(removalListener_, "RemovalListener already assigned");
            removalListener_ = removalListener;
            return this;
        }

        public CacheBuilder<K, V> WithWeigher(IWeigher<K, V> weigher)
        {
            Validate.IsNull(weigher_);
            if(strictParsing_)
            {
                Validate.AreEqual(maximumSize_, UNSET_INT, "weigher can not be combined with maximum size");
            }
            weigher_ = weigher;
            return this;
        }

        public ICache<K, V> Build()
        {
            ValidateWeightWithWeigher();
            ValidateNonLoadingCache();
            return new LocalManualCache<K, V>(this);
        }

        public ILoadingCache<K, V> Build(CacheLoader<K, V> cacheLoader)
        {
            ValidateWeightWithWeigher();
            return new LocalLoadingCache<K, V>(this, cacheLoader);
        }

        private void ValidateNonLoadingCache()
        {
            Validate.AreEqual(refreshTicks_, UNSET_INT, "refreshAfterWrite requires a LoadingCache");
        }

        private void ValidateWeightWithWeigher()
        {
            if(ReferenceEquals(weigher_, null))
            {
                Validate.AreEqual(maximumWeight_, UNSET_INT, "maximumWeight requires a non-null weigher");
                return;
            }

            if(strictParsing_)
            {
                Validate.AreNotEqual(maximumWeight_, UNSET_INT, "weigher requires a maximumWeight");
            }
        }
    }
}
