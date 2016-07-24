using System;
using DxCore.Core.DataStructures;
using DxCore.Core.Utils;
using NUnit.Framework;

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

        [Test]
        public void OverflowHandled()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            for(int i = 0; i < capacity * 4; ++i)
            {
                intBuffer.Add(i);
                Assert.AreEqual(Math.Min(i + 1, capacity), intBuffer.Count);
                for(int j = 0; j <= i; ++j)
                {
                    Assert.AreEqual((i - j) % capacity, intBuffer[j % capacity] % capacity);
                }
            }
        }

        [Test]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void OutOfBounds()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            int shouldThrow = intBuffer[0];
        }

        [Test]
        public void Peek()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);
            CircularBuffer<int> intBuffer = new CircularBuffer<int>(capacity);
            int retrieved;
            bool exists = intBuffer.Peek(out retrieved);
            Assert.IsFalse(exists);

            for(int i = 0; i < capacity * 10; ++i)
            {
                int value = ThreadLocalRandom.Current.Next();
                intBuffer.Add(value);

                exists = intBuffer.Peek(out retrieved);
                Assert.IsTrue(exists);
                Assert.AreEqual(value, retrieved);
            }
        }
    }
}
