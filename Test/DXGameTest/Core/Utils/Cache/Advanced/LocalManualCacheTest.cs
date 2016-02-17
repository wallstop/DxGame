using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DXGame.Core.Utils;
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

            int key = ThreadLocalRandom.Current.Next(3, 5);
            string value = "test value please ignore";
            arbitraryCache.Put(key, value);
            Optional<string> shouldBeCached = arbitraryCache.GetIfPresent(key);
            Assert.True(shouldBeCached.HasValue);
            Assert.AreEqual(value, shouldBeCached.Value);
            Thread.Sleep((int)(writeExpiry.TotalMilliseconds * 2));
            Optional<string> cachedValue = arbitraryCache.GetIfPresent(key);
            if(cachedValue.HasValue)
            {
                Console.WriteLine($"Found value {cachedValue.Value}");
            }
            Assert.False(cachedValue.HasValue);
        }
    }
}
