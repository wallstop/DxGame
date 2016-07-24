using System;
using System.Collections.Generic;
using DxCore.Core.Utils.Cache;
using DxCore.Core.Utils.Cache.Advanced;
using NUnit.Framework;

namespace DXGameTest.Core.Utils.Cache.Advanced
{
    public class LocalManualCacheLruTest
    {
        [Test]
        public void SingleThreadLruExpiration()
        {
            const int maxElements = 10;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string value = "SingleThreadLruExpirationValue";
            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);

                string lruValue;
                bool exists = cache.GetIfPresent(i - maxElements, out lruValue);
                Assert.False(exists);
                Assert.IsTrue(cache.Count <= maxElements);
            }
        }

        [Test]
        public void SingleThreadLruEvictedNotification()
        {
            HashSet<int> expiredKeys = new HashSet<int>();

            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                Assert.AreEqual(RemovalCause.Evicted, notification.RemovalCause);
                bool newKey = expiredKeys.Add(notification.Key);
                Assert.True(newKey);
            };

            const int maxElements = 10;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .WithRemovalListener(removalNotifier)
                    .Build();

            const string value = "SingleThreadLruExpirationNotificationValue";

            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);

                string lruValue;
                int lruKey = i - maxElements;
                bool exists = cache.GetIfPresent(lruKey, out lruValue);
                Assert.False(exists);
                Assert.IsTrue(cache.Count <= maxElements);
                if(0 <= lruKey)
                {
                    Assert.True(expiredKeys.Contains(lruKey));
                }
            }
        }

        [Test]
        public void SingleThreadRandomAccessLruExpiration()
        {
            const int maxElements = 10;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string value = "SingleThreadRandomAccessLruExpirationValue";

            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);

                /* This should make it so that the first elements in the cache are the warmest, always, and will never be removed */
                for(int j = 0; j < i && j < maxElements - 1; ++j)
                {
                    string doesntMatter;
                    bool exists = cache.GetIfPresent(j, out doesntMatter);
                    Assert.True(exists);
                }

                for(int j = maxElements; j < i; ++j)
                {
                    string shouldntExist;
                    bool itExistedOhNo = cache.GetIfPresent(j, out shouldntExist);
                    Assert.False(itExistedOhNo);
                }
            }
        }

        [Test]
        public void LruCacheHandlesDuplicateKeysCorrectly()
        {
            const int maxElements = 10;
            ICache<int, string> cache = CacheBuilder<int, string>.NewBuilder().WithMaxElements(maxElements).Build();

            const string value = "SingleThreadRandomAccessLruExpirationValue";
            const string overriddenValue = "SingleThreadRandomAccessLruExpirationValue2";

            for(int i = 0; i < maxElements * 10; ++i)
            {
                cache.Put(i, value);
                cache.Put(i, overriddenValue);

                /* Cache eviction */
                for(int j = 0; j < i - maxElements && 0 <= j; ++j)
                {
                    string shouldNotExist;
                    bool exists = cache.GetIfPresent(j, out shouldNotExist);
                    Assert.False(exists);
                }

                int minKey = i - maxElements + 1;
                /* Overridden values */
                for(int j = minKey; 0 <= j && j <= i; ++j)
                {
                    string shouldExist;
                    bool exists = cache.GetIfPresent(j, out shouldExist);
                    Assert.True(exists);
                    Assert.AreEqual(shouldExist, overriddenValue);
                }
            }
        }

        [Test]
        public void NoRemovalNotificationOnPutSameValue()
        {
            HashSet<int> expiredKeys = new HashSet<int>();

            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                bool newKey = expiredKeys.Add(notification.Key);
                Assert.True(newKey);
            };

            const int maxElements = 10;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .WithRemovalListener(removalNotifier)
                    .Build();

            const string value = "SingleThreadLruExpirationNotificationValue";

            for(int i = 0; i < maxElements; ++i)
            {
                cache.Put(i, value);
                cache.Put(i, value);

                string foundValue;
                bool exists = cache.GetIfPresent(i, out foundValue);
                Assert.True(exists);
                Assert.AreSame(foundValue, value);
                Assert.IsEmpty(expiredKeys);
            }
        }

        [Test]
        public void LruCacheInsertsElements()
        {
            const int maxElements = 100;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .Build();

            const string valueBase = "testString";

            for (int i = 0; i < maxElements * 10; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);

                string retrievedValue;
                bool success = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsTrue(success);
                Assert.AreSame(value, retrievedValue);

                for (int j = i - maxElements + 1; j <= i && 0 <= j; ++j)
                {
                    string otherCacheValue;
                    bool exists = cache.GetIfPresent(j, out otherCacheValue);
                    Assert.IsTrue(exists);
                    string expected = valueBase + j;
                    Assert.AreEqual(expected, otherCacheValue);
                }
            }
        }

        [Test]
        public void LruCacheInvalidatesElements()
        {
            const int maxElements = 100;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .Build();

            const string valueBase = "testString";

            for(int i = 0; i < maxElements * 10; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);

                string retrievedValue;
                bool success = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsTrue(success);
                Assert.AreSame(value, retrievedValue);

                cache.Invalidate(i);
                bool shouldFail = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsFalse(shouldFail);
            }

            Assert.AreEqual(0, cache.Count);
        }

        [Test]
        public void LruCacheInvalidatesAllElements()
        {
            const int maxElements = 100;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .Build();

            const string valueBase = "testString";

            for(int i = 0; i < maxElements; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);
            }

            cache.InvalidateAll();

            for(int i = 0; i < maxElements; ++i)
            {
                string value;
                bool valueExists = cache.GetIfPresent(i, out value);
                Assert.IsFalse(valueExists);
            }
        }

        [Test]
        public void LruCacheCanCacheElementsAfterFullInvalidation()
        {
            const int maxElements = 100;
            ICache<int, string> cache =
                CacheBuilder<int, string>.NewBuilder()
                    .WithMaxElements(maxElements)
                    .Build();

            const string valueBase = "testString";

            for (int i = 0; i < maxElements; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);
            }

            cache.InvalidateAll();

            for (int i = 0; i < maxElements * 10; ++i)
            {
                string value = valueBase + i;
                cache.Put(i, value);

                string retrievedValue;
                bool success = cache.GetIfPresent(i, out retrievedValue);
                Assert.IsTrue(success);
                Assert.AreSame(value, retrievedValue);
            }
        }
    }
}
