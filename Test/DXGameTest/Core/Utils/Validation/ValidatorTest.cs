using System;
using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using NUnit.Framework;

namespace DXGameTest.Core.Utils.Validation
{
    internal sealed class DoNothingValidator : Validator
    {
        private static readonly Lazy<DoNothingValidator> Singleton = new Lazy<DoNothingValidator>(() => new DoNothingValidator());

        public static DoNothingValidator Instance => Singleton.Value;

        private DoNothingValidator()
        {
        }

        protected override void TestFailure(Func<string> messageProducer)
        {
            // No-op
        }
    }

    public class ValidatorTest
    {
        const string TestMessage = "testMessage";

        private static string TestMessageSupplier()
        {
            return TestMessage;
        }

        [Test]
        public void IsTrue()
        {
            bool validated = DoNothingValidator.Instance.IsTrue(true);
            Assert.IsTrue(validated);
        }

        [Test]
        public void IsTrueWithString()
        {
            bool validated = DoNothingValidator.Instance.IsTrue(true, TestMessage);
            Assert.IsTrue(validated);
        }

        [Test]
        public void IsTrueWithStringSupplier()
        {
            bool validated = DoNothingValidator.Instance.IsTrue(true, TestMessageSupplier);
            Assert.IsTrue(validated);
        }

        [Test]
        public void IsFalse()
        {
            bool validated = DoNothingValidator.Instance.IsFalse(false);
            Assert.IsTrue(validated);
        }

        [Test]
        public void IsFalseWithString()
        {
            bool validated = DoNothingValidator.Instance.IsFalse(false, TestMessage);
            Assert.IsTrue(validated);
        }

        [Test]
        public void IsFalseWithStringSupplier()
        {
            bool validated = DoNothingValidator.Instance.IsFalse(false, TestMessageSupplier);
            Assert.IsTrue(validated);
        }

        [Test]
        public void IsInClosedInterval()
        {
            const int min = -100;
            const int max = 50;
            bool validated = DoNothingValidator.Instance.IsInClosedInterval(min, min, max);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.Instance.IsInClosedInterval(max, min, max);
            Assert.IsTrue(validated);
            for (int i = min; i <= max; ++i)
            {
                validated = DoNothingValidator.Instance.IsInClosedInterval(i, min, max);
                Assert.IsTrue(validated);
            }

            validated = DoNothingValidator.Instance.IsInClosedInterval(min - 1, min, max);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsInClosedInterval(max + 1, min, max);
            Assert.IsFalse(validated);
        }

        [Test]
        public void IsInOpenInterval()
        {
            const int min = -400;
            const int max = -244;
            bool validated = DoNothingValidator.Instance.IsInOpenInterval(min, min, max);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsInOpenInterval(max, min, max);
            Assert.IsFalse(validated);
            for (int i = min + 1; i < max; ++i)
            {
                validated = DoNothingValidator.Instance.IsInOpenInterval(i, min, max);
                Assert.IsTrue(validated);
            }
        }

        [Test]
        public void IsPositive()
        {
            bool validated = DoNothingValidator.Instance.IsPositive(0.1);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.Instance.IsPositive(1);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.Instance.IsPositive(int.MaxValue);
            Assert.IsTrue(validated);
            for (int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(1, int.MaxValue);
                validated = DoNothingValidator.Instance.IsPositive(value);
                Assert.IsTrue(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(0.0000001, double.MaxValue);
                validated = DoNothingValidator.Instance.IsPositive(value);
                Assert.IsTrue(validated);
            }

            validated = DoNothingValidator.Instance.IsPositive(-0.1);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsPositive(0.0);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsPositive(-0.0);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsPositive(-1111.4);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsPositive(int.MinValue);
            Assert.IsFalse(validated);

            for (int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(int.MinValue, 0);
                validated = DoNothingValidator.Instance.IsPositive(value);
                Assert.IsFalse(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(double.MinValue, 0);
                validated = DoNothingValidator.Instance.IsPositive(value);
                Assert.IsFalse(validated);
            }
        }

        [Test]
        public void IsNegative()
        {
            bool validated = DoNothingValidator.Instance.IsNegative(0.1);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsNegative(1);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsNegative(int.MaxValue);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsNegative(0.0);
            Assert.IsFalse(validated);
            validated = DoNothingValidator.Instance.IsNegative(-0.0);
            Assert.IsFalse(validated);
            for(int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(0, int.MaxValue);
                validated = DoNothingValidator.Instance.IsNegative(value);
                Assert.IsFalse(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(0, double.MaxValue);
                validated = DoNothingValidator.Instance.IsNegative(value);
                Assert.IsFalse(validated);
            }

            validated = DoNothingValidator.Instance.IsNegative(-0.1);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.Instance.IsNegative(-0.00000001);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.Instance.IsNegative(-1111.4);
            Assert.IsTrue(validated);
            validated = DoNothingValidator.Instance.IsNegative(int.MinValue);
            Assert.IsTrue(validated);

            for(int i = 0; i < 100; ++i)
            {
                int value = ThreadLocalRandom.Current.Next(int.MinValue, -1);
                validated = DoNothingValidator.Instance.IsNegative(value);
                Assert.IsTrue(validated);
            }
            for(int i = 0; i < 100; ++i)
            {
                double value = ThreadLocalRandom.Current.NextDouble(double.MinValue, -0);
                validated = DoNothingValidator.Instance.IsNegative(value);
                Assert.IsTrue(validated);
            }
        }

        [Test]
        public void IsElementOf()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);

            List<int> elements =
                Enumerable.Range(0, capacity).Select(_ => ThreadLocalRandom.Current.Next()).ToList();

            foreach (int element in elements)
            {
                bool validated = DoNothingValidator.Instance.IsElementOf(elements, element);
                Assert.IsTrue(validated);
            }
            for (int i = 0; i < capacity; ++i)
            {
                int generatedElement;
                do
                {
                    generatedElement = ThreadLocalRandom.Current.Next();
                } while (elements.Contains(generatedElement));
                bool validated = DoNothingValidator.Instance.IsElementOf(elements, generatedElement);
                Assert.IsFalse(validated);
            }
        }

        [Test]
        public void IsNotElementOf()
        {
            int capacity = ThreadLocalRandom.Current.Next(10, 50);

            List<int> elements =
                Enumerable.Range(0, capacity).Select(_ => ThreadLocalRandom.Current.Next()).ToList();

            foreach(int element in elements)
            {
                bool validated = DoNothingValidator.Instance.IsNotElementOf(elements, element);
                Assert.IsFalse(validated);
            }
            for(int i = 0; i < capacity; ++i)
            {
                int generatedElement;
                do
                {
                    generatedElement = ThreadLocalRandom.Current.Next();
                } while(elements.Contains(generatedElement));
                bool validated = DoNothingValidator.Instance.IsNotElementOf(elements, generatedElement);
                Assert.IsTrue(validated);
            }
        }
    }
}
