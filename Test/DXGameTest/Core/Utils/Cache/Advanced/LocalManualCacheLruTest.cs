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
        public void SingleThreadLruExpirationNotification()
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
                for(int j = 0; j < i && j < (maxElements - 1); ++j)
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
    }
}
