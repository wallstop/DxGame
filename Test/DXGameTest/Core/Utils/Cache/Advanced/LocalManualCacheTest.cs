using System;
using System.Threading;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Cache;
using DXGame.Core.Utils.Cache.Advanced;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Utils.Cache.Advanced
{
    public class LocalManualCacheTest
    {
        [Test]
        public void TestExpireAfterWriteRespectsTimeout()
        {
            TimeSpan writeExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(1000, 1500));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithExpireAfterWrite(writeExpiry).Build();

            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";
            arbitraryCache.Put(key, value);
            Optional<string> shouldBeCached = arbitraryCache.GetIfPresent(key);
            Assert.True(shouldBeCached.HasValue);
            Assert.AreEqual(value, shouldBeCached.Value);
            Thread.Sleep((int) (writeExpiry.TotalMilliseconds * 2));
            Optional<string> cachedValue = arbitraryCache.GetIfPresent(key);
            Assert.False(cachedValue.HasValue);
        }

        [Test]
        public void TestExpireAfterAccessRespectsTimeout()
        {
            TimeSpan accessExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(1000, 1500));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithExpireAfterAccess(accessExpiry).Build();

            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";
            arbitraryCache.Put(key, value);
            for(int i = 0; i < ThreadLocalRandom.Current.Next(5, 20); ++i)
            {
                Optional<string> shouldBeCached = arbitraryCache.GetIfPresent(key);
                Assert.True(shouldBeCached.HasValue);
                Assert.AreEqual(value, shouldBeCached.Value);
                /* Sleep for a little bit */
                Thread.Sleep((int) (accessExpiry.TotalMilliseconds / 4));
            }
            /* Sleep for a lot */
            Thread.Sleep((int) (accessExpiry.TotalMilliseconds * 2));
            Optional<string> cachedValue = arbitraryCache.GetIfPresent(key);
            Assert.False(cachedValue.HasValue);
        }

        [Test]
        public void TestRemovalNotificationReplacedEntry()
        {
            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.False(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Replaced, notification.RemovalCause);
            };
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).Build();

            arbitraryCache.Put(key, value);

            string differentValue = "other test value please ignore me too";
            arbitraryCache.Put(key, differentValue);
            Assert.True(removalCalled);
        }

        [Test]
        public void TestNoRemovalNotificationSameEntry()
        {
            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                Assert.Fail("Removal notification was called when calling put with same key value pair");
            };

            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).Build();

            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";
            arbitraryCache.Put(key, value);
            arbitraryCache.Put(key, value);
            Assert.False(removalCalled);
        }

        [Test]
        public void TestRemovalNotificationExplicitInvalidate()
        {
            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.False(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Explicit, notification.RemovalCause);
            };
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).Build();

            arbitraryCache.Put(key, value);
            arbitraryCache.Invalidate(key);
            Assert.True(removalCalled);
        }

        [Test]
        public void TestRemovalNotificationExpiredWriteTimeout()
        {
            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.False(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Expired, notification.RemovalCause);
            };

            TimeSpan writeExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(1000, 1500));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).WithExpireAfterWrite(writeExpiry).Build();

            arbitraryCache.Put(key, value);
            Thread.Sleep((int) writeExpiry.TotalMilliseconds * 2);
            Optional<string> shouldNotExist = arbitraryCache.GetIfPresent(key);
            Assert.False(shouldNotExist.HasValue);
            /* Force a removal notification via a "modification" operation */
            arbitraryCache.Put(key, value + "other value");
            Assert.True(removalCalled);
        }

        [Test]
        public void TestRemovalNotificationExpiredAccessTimeout()
        {
            int key = ThreadLocalRandom.Current.Next();
            string value = "test value please ignore";

            bool removalCalled = false;
            Action<RemovalNotification<int, string>> removalNotifier = notification =>
            {
                /* We should only be called once */
                Assert.False(removalCalled);
                removalCalled = true;
                Assert.AreEqual(key, notification.Key);
                Assert.AreEqual(value, notification.Value);
                Assert.AreEqual(RemovalCause.Expired, notification.RemovalCause);
            };

            TimeSpan accessExpiry = TimeSpan.FromMilliseconds(ThreadLocalRandom.Current.Next(1000, 1500));
            ICache<int, string> arbitraryCache =
                CacheBuilder<int, string>.NewBuilder().WithRemovalListener(removalNotifier).WithExpireAfterAccess(accessExpiry).Build();

            arbitraryCache.Put(key, value);
            Optional<string> maybeValue = arbitraryCache.GetIfPresent(key);
            Assert.True(maybeValue.HasValue);
            Assert.AreEqual(value, maybeValue.Value);
            Thread.Sleep((int) accessExpiry.TotalMilliseconds * 2);
            Optional<string> shouldNotExist = arbitraryCache.GetIfPresent(key);
            Assert.False(shouldNotExist.HasValue);
            /* Force a removal notification via a "modification" operation */
            arbitraryCache.Put(key, value + "other value");
            Assert.True(removalCalled);
        }
    }
}
