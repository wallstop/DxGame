using DXGame.Core.Properties;
using NUnit.Framework;

namespace DXGameTest.Core.Property
{
    public class PropertyMutatorComparer
    {
        [Test]
        public void NullComparisons()
        {
            PropertyMutator<int> highPriorityMutator =
                new PropertyMutator<int>(SimpleMutator, SimpleDeMutator,
                    "SimplePropertyMutator", MutatePriority.High);
            PropertyMutator<int> mediumPriorityMutator =
                new PropertyMutator<int>(SimpleMutator, SimpleDeMutator,
                    "SimplePropertyMutator", MutatePriority.Medium);
            PropertyMutator<int> lowPriorityMutator = new PropertyMutator<int>(
                SimpleMutator, SimpleDeMutator, "SimplePropertyMutator",
                MutatePriority.Low);

            var priorityComparer = new PropertyMutatorPriorityComparer<int>();

            Assert.Less(0, priorityComparer.Compare(highPriorityMutator, null));
            Assert.Less(0, priorityComparer.Compare(mediumPriorityMutator, null));
            Assert.Less(0, priorityComparer.Compare(lowPriorityMutator, null));

            Assert.Greater(0, priorityComparer.Compare(null, highPriorityMutator));
            Assert.Greater(0, priorityComparer.Compare(null, mediumPriorityMutator));
            Assert.Greater(0, priorityComparer.Compare(null, lowPriorityMutator));
        }

        [Test]
        public void PriorityComparison()
        {
            PropertyMutator<int> highPriorityMutator =
                new PropertyMutator<int>(SimpleMutator, SimpleDeMutator,
                    "SimplePropertyMutator", MutatePriority.High);
            PropertyMutator<int> mediumPriorityMutator =
                new PropertyMutator<int>(SimpleMutator, SimpleDeMutator,
                    "SimplePropertyMutator", MutatePriority.Medium);
            PropertyMutator<int> lowPriorityMutator = new PropertyMutator<int>(
                SimpleMutator, SimpleDeMutator, "SimplePropertyMutator",
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

        private static int SimpleMutator(int input) { return input + 2; }
        private static int SimpleDeMutator(int input) { return input - 2; }
    }
}