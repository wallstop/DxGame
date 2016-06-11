using System;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Cache;
using DxCore.Core.Utils.Cache.Advanced;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.Utils.Cache.Advanced
{
    public class CacheBuilderTest
    {
        [Test]
        public void TestExpireAfterAccessUnsetTurnsToDefault()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Assert.AreEqual(0, cacheBuilder.ExpireAfterAccessTicks);
        }

        [Test]
        public void TestExpireAfterWriteUnsetTurnsToDefault()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Assert.AreEqual(0, cacheBuilder.ExpireAfterWriteTicks);
        }

        [Test]
        public void TestRemovalListenerUnsetIsNull()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Assert.IsNull(cacheBuilder.RemovalListener);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidExpireAfterAccessThrows()
        {
            CacheBuilder<int, string>.NewBuilder().WithExpireAfterAccess(TimeSpan.FromSeconds(-1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidExpireAfterWriteThrows()
        {
            CacheBuilder<int, string>.NewBuilder().WithExpireAfterWrite(TimeSpan.FromSeconds(-1));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidExpireNullRemovalListenerThrows()
        {
            CacheBuilder<int, string>.NewBuilder().WithRemovalListener(null);
        }

        [Test]
        public void TestRemovalListenerSet()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            Action<RemovalNotification<int, string>> notification = removalNotification => { };
            cacheBuilder.WithRemovalListener(notification);
            Assert.NotNull(cacheBuilder.RemovalListener);
            Assert.AreEqual(notification, cacheBuilder.RemovalListener);
        }

        [Test]
        public void TestExpireAfterAccessSet()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            TimeSpan expireAfterAccessSeconds = TimeSpan.FromSeconds(ThreadLocalRandom.Current.Next());
            cacheBuilder.WithExpireAfterAccess(expireAfterAccessSeconds);
            Assert.AreEqual(expireAfterAccessSeconds.Ticks, cacheBuilder.ExpireAfterAccessTicks);
        }

        [Test]
        public void TestExpireAfterWriteSet()
        {
            CacheBuilder<int, string> cacheBuilder = CacheBuilder<int, string>.NewBuilder();
            TimeSpan expireAfterWriteSeconds = TimeSpan.FromSeconds(ThreadLocalRandom.Current.Next());
            cacheBuilder.WithExpireAfterAccess(expireAfterWriteSeconds);
            Assert.AreEqual(expireAfterWriteSeconds.Ticks, cacheBuilder.ExpireAfterAccessTicks);
        }
    }
}
