using DxCore.Core.DataStructures;
using DxCore.Core.Utils;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace DXGameTest.Core.DataStructures
{
    public class CircularBufferTest
    {
        [Test]
        public void SimpleBufferTest()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            for(int i = 0; i < capacity; ++i)
            {
                intBuffer.Add(i);
                Assert.AreEqual(i + 1, intBuffer.Count);
                for(int j = 0; j <= i; ++j)
                {
                    Assert.AreEqual(i - j, intBuffer[j]);
                }
            }
        }
    }
}
