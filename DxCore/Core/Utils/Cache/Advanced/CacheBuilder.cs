using System;

namespace DxCore.Core.Utils.Cache.Advanced
{
    public class CacheBuilder<K, V>
    {
        private const int UnsetInt = -1;

        private long expireAfterAccessMilliseconds_ = UnsetInt;
        private long expireAfterWriteMilliseconds_ = UnsetInt;
        private long maxElements_ = UnsetInt;

        public long? ExpireAfterAccessMilliseconds
        {
            get
            {
                if(expireAfterAccessMilliseconds_ == UnsetInt)
                {
                    return null;
                }
                return expireAfterAccessMilliseconds_;
            }
        }

        public long? ExpireAfterWriteMilliseconds
        {
            get
            {
                if(expireAfterWriteMilliseconds_ == UnsetInt)
                {
                    return null;
                }
                return expireAfterWriteMilliseconds_;
            }
        }

        public long? MaxElements
        {
            get
            {
                if(maxElements_ == UnsetInt)
                {
                    return null;
                }
                return maxElements_;
            }
        }

        public Action<RemovalNotification<K, V>> RemovalListener { get; private set; }

        private CacheBuilder() {}

        public ICache<K, V> Build()
        {
            return new LocalCache<K, V>(this);
        }

        public ILoadingCache<K, V> Build(Func<K, V> valueLoader)
        {
            return new LocalLoadingCache<K, V>(this, valueLoader);
        }

        public ILoadingCache<K, V> Build(Func<V> valueLoader)
        {
            return new LocalLoadingCache<K, V>(this, valueLoader);
        }

        public static CacheBuilder<K, V> NewBuilder()
        {
            return new CacheBuilder<K, V>();
        }

        public CacheBuilder<K, V> WithExpireAfterAccess(TimeSpan duration)
        {
            long durationMilliseconds = (long) Math.Round(duration.TotalMilliseconds);
            Validate.Validate.Hard.IsNotNegative(durationMilliseconds,
                () => $"Cannot expire after access of a duration of {durationMilliseconds}ms");
            expireAfterAccessMilliseconds_ = durationMilliseconds;
            return this;
        }

        public CacheBuilder<K, V> WithExpireAfterWrite(TimeSpan duration)
        {
            long durationMilliseconds = (long) Math.Round(duration.TotalMilliseconds);
            Validate.Validate.Hard.IsNotNegative(durationMilliseconds,
                () => $"Cannot expire after write of a duration of {durationMilliseconds}ms");
            expireAfterWriteMilliseconds_ = durationMilliseconds;
            return this;
        }

        public CacheBuilder<K, V> WithMaxElements(int maxElements)
        {
            Validate.Validate.Hard.IsPositive(maxElements,
                () => $"Cannot cap the max elements of a cache to be {maxElements}");
            maxElements_ = maxElements;
            return this;
        }

        public CacheBuilder<K, V> WithRemovalListener(Action<RemovalNotification<K, V>> removalListener)
        {
            Validate.Validate.Hard.IsNotNull(removalListener, "Cannot register a null removalListener");
            RemovalListener = removalListener;
            return this;
        }
    }
}