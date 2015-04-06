﻿using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using DXGame.Core.Properties;
using DXGame.Core.Utils;
using NUnit.Framework;

namespace DXGameTest.Core.Property
{
    public class PropertyMutation
    {
        [Test]
        public void SimpleMutation()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");

            const int original = 200;
            const int expected = 400;
            int mutateOutcome = intMutator.Mutate(original);
            Assert.AreEqual(expected, mutateOutcome);
            int demutateOutcome = intMutator.DeMutate(mutateOutcome);
            Assert.AreEqual(original, demutateOutcome);
        }

        [Test]
        public void MultipleMutations()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");
            intMutator.Count = 5;

            const int original = 200;
            // Since we've increased the count, we should be expecting a much larger number than the original (5x larger, in fact!)
            const int expected = 200 * 2 * 2 * 2 * 2 * 2;
            int mutateOutcome = intMutator.Mutate(original);
            Assert.AreEqual(expected, mutateOutcome);
            int demutateOutcome = intMutator.DeMutate(mutateOutcome);
            Assert.AreEqual(original, demutateOutcome);
        }

        [Test]
        public void NullEquality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");
            Assert.True(intMutator != null);
            Assert.True(null != intMutator);
            Assert.False(intMutator.Equals(null));
            Assert.False(intMutator.Equals((PropertyMutator<int>)null));
            Assert.False(intMutator.Equals((object)null));
            Assert.False(intMutator == null);
            Assert.False(null == intMutator);
            Assert.True((PropertyMutator<int>)null == (PropertyMutator<int>) null);
            Assert.AreNotEqual(intMutator, null);
            Assert.AreNotEqual(null, intMutator);
        }

        [Test]
        public void DifferentMutatorAndDemutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2, SimpleDeMutatorVersion2, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void DifferentMutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
            Assert.True(intMutator != otherIntMutator);
            Assert.True(otherIntMutator != intMutator);
        }

        [Test]
        public void DifferetDemutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion2, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
            Assert.True(intMutator != otherIntMutator);
            Assert.True(otherIntMutator != intMutator);
        }

        [Test]
        public void DifferentName()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            // Note the "e" in "Mutater"
            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutater");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void DifferentPriority()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.High);

            // Note the "e" in "Mutater"
            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutater", Mutat);

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void Equality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            Assert.AreEqual(intMutator, otherIntMutator);
            Assert.AreEqual(otherIntMutator, intMutator);

            // Make sure counts don't interfere with equality
            otherIntMutator.Count = 500;
            Assert.AreEqual(intMutator, otherIntMutator);
            Assert.AreEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void HashCode()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");

            Assert.AreEqual(intMutator.GetHashCode(), otherIntMutator.GetHashCode());
            Assert.AreEqual(otherIntMutator.GetHashCode(), intMutator.GetHashCode());

            // Make sure counts don't interfere with equality
            otherIntMutator.Count = 500;
            Assert.AreEqual(intMutator.GetHashCode(), otherIntMutator.GetHashCode());
            Assert.AreEqual(otherIntMutator.GetHashCode(), intMutator.GetHashCode());

            // Check different demutator
            PropertyMutator<int> differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion2, "SimplePropertyMutator");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different mutator
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2, SimpleMutatorVersion1, "SimplePropertyMutator");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different name
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SomeOtherName");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());   
        }

        [Test]
        public void ToStringContainsName()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator");
            Assert.True(intMutator.ToString().Contains("SimplePropertyMutator"));
        }

        [Test]
        public void PriorityComparison()
        {
            PropertyMutator<int> highPriorityMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.High);
            PropertyMutator<int> mediumPriorityMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.Medium);
            PropertyMutator<int> lowPriorityMutator = new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.Low);

            // All mutators at the same priority should compare equal (0)
            Assert.AreEqual(0, PropertyMutator<int>.PriorityComparison(highPriorityMutator, highPriorityMutator));
            Assert.AreEqual(0, PropertyMutator<int>.PriorityComparison(mediumPriorityMutator, mediumPriorityMutator));
            Assert.AreEqual(0, PropertyMutator<int>.PriorityComparison(lowPriorityMutator, lowPriorityMutator));

            // All less-than comparisons should result in a value greater than 0 (hence, 0 is less than the value)
            // Due to construction, "High Priority" should compare to be smaller than all other priorities, thus resulting in being sorted first
            Assert.Less(0, PropertyMutator<int>.PriorityComparison(mediumPriorityMutator, highPriorityMutator));
            Assert.Less(0, PropertyMutator<int>.PriorityComparison(lowPriorityMutator, mediumPriorityMutator));
            Assert.Less(0, PropertyMutator<int>.PriorityComparison(lowPriorityMutator, highPriorityMutator));

            // All greater-than comparisons should result in a value less than 0 (hence, 0 is greater than the value)
            // Due to construction "Low Priority" should compare to be larger than all other priority, thus resulting in being sorted last
            Assert.Greater(0, PropertyMutator<int>.PriorityComparison(highPriorityMutator, mediumPriorityMutator));
            Assert.Greater(0, PropertyMutator<int>.PriorityComparison(highPriorityMutator, lowPriorityMutator));
            Assert.Greater(0, PropertyMutator<int>.PriorityComparison(mediumPriorityMutator, lowPriorityMutator));
        }

        private static int SimpleMutatorVersion1(int input)
        {
            return input * 2;
        }

        private static int SimpleDeMutatorVersion1(int input)
        {
            return input / 2;
        }

        private static int SimpleMutatorVersion2(int input)
        {
            return input * 3;
        }

        private static int SimpleDeMutatorVersion2(int input)
        {
            return input / 3;
        }
    }
}