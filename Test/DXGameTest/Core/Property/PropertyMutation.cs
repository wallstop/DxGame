using System;
using DXGame.Core.Properties;
using NUnit.Framework;

namespace DXGameTest.Core.Property
{
    public class PropertyMutation
    {
        [Test]
        public void SimpleMutation()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>((input, count) => input * 2, "SimplePropertyMutator");

            const int original = 200;
            const int expected = 400;
            int mutateOutcome = intMutator.Mutate(original, 1);
            Assert.AreEqual(expected, mutateOutcome);
        }

        [Test]
        public void MultipleMutations()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>((input, count) => input * (int)Math.Pow(2, count), "SimplePropertyMutator");

            const int original = 200;
            // Since we've increased the count, we should be expecting a much larger number than the original (5x larger, in fact!)
            int expected = 200 * (int) Math.Pow(2, 5);
            int mutateOutcome = intMutator.Mutate(original, 5);
            Assert.AreEqual(expected, mutateOutcome);
        }

        [Test]
        public void NullEquality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");
            Assert.True(intMutator != null);
            Assert.True(null != intMutator);
            Assert.False(intMutator.Equals(null));
            Assert.False(intMutator.Equals((object) null));
            Assert.False(intMutator == null);
            Assert.False(null == intMutator);
            Assert.True(null == (PropertyMutator<int>) null);
            Assert.AreNotEqual(intMutator, null);
            Assert.AreNotEqual(null, intMutator);
        }

        [Test]
        public void OtherObjectEquality()
        {
            // Make sure we aren't accidentally equal with any other kinds of objects...
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");
            PropertyMutator<string> stringMutator = new PropertyMutator<string>((input, count) => input, "SimplePropertyMutator");
            PropertyMutator<long> otherIntMutator = new PropertyMutator<long>((input, count) => input, "SimplePropertyMutator");

            Assert.False(intMutator.Equals(stringMutator));
            Assert.False(intMutator.Equals(otherIntMutator));
            Assert.False(otherIntMutator.Equals(4));
            Assert.False(otherIntMutator.Equals(new string[4]));
        }

        [Test]
        public void DifferentMutatorAndDemutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2,
                "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void DifferentMutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2,
                "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
            Assert.True(intMutator != otherIntMutator);
            Assert.True(otherIntMutator != intMutator);
        }

        [Test]
        public void DifferentName()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");

            // Note the "e" in "Mutater"
            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                "SimplePropertyMutater");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void DifferentPriority()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator",
                MutatePriority.High);
            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                "SimplePropertyMutator", MutatePriority.Low);

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void Equality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                "SimplePropertyMutator");

            Assert.AreEqual(intMutator, otherIntMutator);
            Assert.AreEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void HashCode()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                "SimplePropertyMutator");

            Assert.AreEqual(intMutator.GetHashCode(), otherIntMutator.GetHashCode());
            Assert.AreEqual(otherIntMutator.GetHashCode(), intMutator.GetHashCode());

            // Check different mutator
            PropertyMutator<int>  differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2, "SimplePropertyMutator");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different name
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SomeOtherName");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different MutatePriority
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator",
                MutatePriority.Low);
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());
        }

        [Test]
        public void ToStringContainsName()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, "SimplePropertyMutator");
            Assert.True(intMutator.ToString().Contains("SimplePropertyMutator"));
        }

        private static int SimpleMutatorVersion1(int input, int count)
        {
            return input * 2;
        }

        private static int SimpleMutatorVersion2(int input, int count)
        {
            return input * 3;
        }
    }
}