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
            PropertyMutator<int> intMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");

            const int original = 200;
            const int expected = 400;
            int mutateOutcome = intMutator.Mutate(original, 1);
            Assert.AreEqual(expected, mutateOutcome);
            int demutateOutcome = intMutator.DeMutate(mutateOutcome, 1);
            Assert.AreEqual(original, demutateOutcome);
        }

        [Test]
        public void MultipleMutations()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");

            const int original = 200;
            // Since we've increased the count, we should be expecting a much larger number than the original (5x larger, in fact!)
            int expected = 200 * (int) (Math.Pow(2, 5));
            int mutateOutcome = intMutator.Mutate(original, 5);
            Assert.AreEqual(expected, mutateOutcome);
            int demutateOutcome = intMutator.DeMutate(mutateOutcome, 5);
            Assert.AreEqual(original, demutateOutcome);
        }

        [Test]
        public void NullEquality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");
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
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");
            PropertyMutator<string> stringMutator = new PropertyMutator<string>(input => input,
                input => input, "SimplePropertyMutator");
            PropertyMutator<Int64> otherIntMutator =
                new PropertyMutator<Int64>(input => input, input => input,
                    "SimplePropertyMutator");

            Assert.False(intMutator.Equals(stringMutator));
            Assert.False(intMutator.Equals(otherIntMutator));
            Assert.False(otherIntMutator.Equals(4));
            Assert.False(otherIntMutator.Equals(new string[4]));
        }

        [Test]
        public void DifferentMutatorAndDemutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2,
                SimpleDeMutatorVersion2, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void DifferentMutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
            Assert.True(intMutator != otherIntMutator);
            Assert.True(otherIntMutator != intMutator);
        }

        [Test]
        public void DifferetDemutator()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion2, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
            Assert.True(intMutator != otherIntMutator);
            Assert.True(otherIntMutator != intMutator);
        }

        [Test]
        public void DifferentName()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            // Note the "e" in "Mutater"
            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutater");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void DifferentPriority()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.High);
            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.Low);

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void Equality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            Assert.AreEqual(intMutator, otherIntMutator);
            Assert.AreEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void HashCode()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");

            Assert.AreEqual(intMutator.GetHashCode(), otherIntMutator.GetHashCode());
            Assert.AreEqual(otherIntMutator.GetHashCode(), intMutator.GetHashCode());

            // Check different demutator
            PropertyMutator<int> differentIntMutator =
                new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion2,
                    "SimplePropertyMutator");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different mutator
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion2,
                SimpleMutatorVersion1, "SimplePropertyMutator");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different name
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SomeOtherName");
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());

            // Check different MutatePriority
            differentIntMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator", MutatePriority.Low);
            Assert.AreNotEqual(intMutator.GetHashCode(), differentIntMutator.GetHashCode());
        }

        [Test]
        public void ToStringContainsName()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(SimpleMutatorVersion1,
                SimpleDeMutatorVersion1, "SimplePropertyMutator");
            Assert.True(intMutator.ToString().Contains("SimplePropertyMutator"));
        }

        [Test]
        public void PriorityComparison()
        {
            PropertyMutator<int> highPriorityMutator =
                new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1,
                    "SimplePropertyMutator", MutatePriority.High);
            PropertyMutator<int> mediumPriorityMutator =
                new PropertyMutator<int>(SimpleMutatorVersion1, SimpleDeMutatorVersion1,
                    "SimplePropertyMutator", MutatePriority.Medium);
            PropertyMutator<int> lowPriorityMutator = new PropertyMutator<int>(
                SimpleMutatorVersion1, SimpleDeMutatorVersion1, "SimplePropertyMutator",
                MutatePriority.Low);

            var priorityComparer = new PropertyMutatorPriorityComparer<int>();

            // All mutators at the same priority should compare equal (0)
            Assert.AreEqual(0,
                priorityComparer.Compare(highPriorityMutator, highPriorityMutator));
            Assert.AreEqual(0,
                priorityComparer.Compare(mediumPriorityMutator, mediumPriorityMutator));
            Assert.AreEqual(0,
                priorityComparer.Compare(lowPriorityMutator, lowPriorityMutator));

            // All less-than comparisons should result in a value greater than 0 (hence, 0 is less than the value)
            // Due to construction, "High Priority" should compare to be smaller than all other priorities, thus resulting in being sorted first
            Assert.Less(0,
                priorityComparer.Compare(mediumPriorityMutator, highPriorityMutator));
            Assert.Less(0,
                priorityComparer.Compare(lowPriorityMutator, mediumPriorityMutator));
            Assert.Less(0,
                priorityComparer.Compare(lowPriorityMutator, highPriorityMutator));

            // All greater-than comparisons should result in a value less than 0 (hence, 0 is greater than the value)
            // Due to construction "Low Priority" should compare to be larger than all other priority, thus resulting in being sorted last
            Assert.Greater(0,
                priorityComparer.Compare(highPriorityMutator, mediumPriorityMutator));
            Assert.Greater(0,
                priorityComparer.Compare(highPriorityMutator, lowPriorityMutator));
            Assert.Greater(0,
                priorityComparer.Compare(mediumPriorityMutator, lowPriorityMutator));
        }

        private static int SimpleMutatorVersion1(int input) { return input * 2; }
        private static int SimpleDeMutatorVersion1(int input) { return input / 2; }
        private static int SimpleMutatorVersion2(int input) { return input * 3; }
        private static int SimpleDeMutatorVersion2(int input) { return input / 3; }
    }
}