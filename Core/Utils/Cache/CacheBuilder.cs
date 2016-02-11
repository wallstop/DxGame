using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Utils.Cache.Simple;

namespace DXGame.Core.Utils.Cache
{
    public sealed class CacheBuilder<K, V>
    {
        private static readonly int UNSET_INT = -1;

        private const int DEFAULT_INITIAL_CAPACITY = 16;
        private const int DEFAULT_CONCURRENCY_LEVEL = 4;
        private const int DEFAULT_EXPIRATION_NANOS = 0;
        private const int DEFAULT_REFRESH_NANOS = 0;

        private bool strictParsing_ = false;
        private int initialCapacity_ = UNSET_INT;
        private int concurrenyLevel_ = UNSET_INT;
        private long maximumSize_ = UNSET_INT;
        private long maximumWeight_ = UNSET_INT;

        private IWeigher<K, V> weigher_;

        private long expireAfterWriteTicks_ = UNSET_INT;
        private long expireAfterAccessTicks_ = UNSET_INT;
        private long refreshTicks_ = UNSET_INT;

        public long ExpireAfterWrite
            => (expireAfterWriteTicks_ == UNSET_INT) ? DEFAULT_REFRESH_NANOS : expireAfterWriteTicks_;

        public long ExpireAfterAccess
            => (expireAfterAccessTicks_ == UNSET_INT) ? DEFAULT_REFRESH_NANOS : expireAfterAccessTicks_;

        public long Refresh => (refreshTicks_ == UNSET_INT) ? DEFAULT_REFRESH_NANOS : refreshTicks_;

        private IRemovalListener<K, V> removalListener_;

        public IRemovalListener<K, V> RemovalListener
            => (ReferenceEquals(removalListener_, null) ? NullRemovalListener<K, V>.Instance : removalListener_);

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

        public ICache<K, V> Build()
        {
            // TODO
            return null;
        } 

    }
}
