using System;
using DXGame.Core.Utils;
using NUnit.Framework;

namespace DXGameTest.Core.Utils
{
    public class MathUtilsTest
    {
        [Test]
        public void TestMax()
        {
            int small = -100;
            int medium = 13;
            int large = 100002;

            // Make sure Max works with the basic numeric types
            Max(small, medium, large);
            Max<float>(small, medium, large);
            Max<double>(small, medium, large);
            Max<long>(small, medium, large);
            Max<Int64>(small, medium, large);
        }

        [Test]
        public void TestMin()
        {
            int small = -100;
            int medium = 13;
            int large = 100002;

            // Make sure Min works with the basic numeric types
            Min(small, medium, large);
            Min<float>(small, medium, large);
            Min<double>(small, medium, large);
            Min<long>(small, medium, large);
            Min<Int64>(small, medium, large);
        }

        [Test]
        public void TestCompare()
        {
            int small = -100;
            int medium = 13;
            int large = 100002;

            // Make sure Compare works with the basic numeric types
            Compare(small, medium, large);
            Compare<float>(small, medium, large);
            Compare<double>(small, medium, large);
            Compare<long>(small, medium, large);
            Compare<Int64>(small, medium, large);
        }

        [Test]
        public void TestSignOf()
        {
            int negative = -100;
            int positive = 100;
            int zero = 0;

            Assert.AreEqual(1, MathUtils.SignOf(positive));
            Assert.AreEqual(1, MathUtils.SignOf(zero));
            Assert.AreEqual(-1, MathUtils.SignOf(negative));

            Assert.AreEqual(1, MathUtils.SignOf<float>(positive));
            Assert.AreEqual(1, MathUtils.SignOf<float>(zero));
            Assert.AreEqual(-1, MathUtils.SignOf<float>(negative));

            Assert.AreEqual(1, MathUtils.SignOf<double>(positive));
            Assert.AreEqual(1, MathUtils.SignOf<double>(zero));
            Assert.AreEqual(-1, MathUtils.SignOf<double>(negative));

            Assert.AreEqual(1, MathUtils.SignOf<Int64>(positive));
            Assert.AreEqual(1, MathUtils.SignOf<Int64>(zero));
            Assert.AreEqual(-1, MathUtils.SignOf<Int64>(negative));

            Assert.AreEqual(1, MathUtils.SignOf<long>(positive));
            Assert.AreEqual(1, MathUtils.SignOf<long>(zero));
            Assert.AreEqual(-1, MathUtils.SignOf<long>(negative));
        }

        private static void Max<T>(T small, T medium, T large)
        {
            Assert.AreEqual(small, MathUtils.Max(small, small));
            Assert.AreEqual(medium, MathUtils.Max(medium, small));
            Assert.AreEqual(medium, MathUtils.Max(small, medium));
            Assert.AreEqual(medium, MathUtils.Max(medium, medium));
            Assert.AreEqual(large, MathUtils.Max(large, small));
            Assert.AreEqual(large, MathUtils.Max(small, large));
            Assert.AreEqual(large, MathUtils.Max(large, medium));
            Assert.AreEqual(large, MathUtils.Max(medium, large));
            Assert.AreEqual(large, MathUtils.Max(large, large));
        }

        private static void Min<T>(T small, T medium, T large)
        {
            Assert.AreEqual(small, MathUtils.Min(small, small));
            Assert.AreEqual(small, MathUtils.Min(medium, small));
            Assert.AreEqual(small, MathUtils.Min(small, medium));
            Assert.AreEqual(medium, MathUtils.Min(medium, medium));
            Assert.AreEqual(small, MathUtils.Min(large, small));
            Assert.AreEqual(small, MathUtils.Min(small, large));
            Assert.AreEqual(medium, MathUtils.Min(large, medium));
            Assert.AreEqual(medium, MathUtils.Min(medium, large));
            Assert.AreEqual(large, MathUtils.Min(large, large));
        }

        private static void Compare<T>(T small, T medium, T large)
        {
            Assert.AreEqual(0, MathUtils.Compare(small, small));
            Assert.AreEqual(1, MathUtils.Compare(medium, small));
            Assert.AreEqual(-1, MathUtils.Compare(small, medium));
            Assert.AreEqual(0, MathUtils.Compare(medium, medium));
            Assert.AreEqual(1, MathUtils.Compare(large, small));
            Assert.AreEqual(-1, MathUtils.Compare(small, large));
            Assert.AreEqual(1, MathUtils.Compare(large, medium));
            Assert.AreEqual(-1, MathUtils.Compare(medium, large));
            Assert.AreEqual(0, MathUtils.Compare(large, large));
        }
    }
}