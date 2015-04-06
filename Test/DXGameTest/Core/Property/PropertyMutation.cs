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
        public void Inequality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(input => input * 3,
                input => input / 3, "SimplePropertyMutator");

            Assert.AreNotEqual(intMutator, otherIntMutator);
            Assert.AreNotEqual(otherIntMutator, intMutator);
        }

        [Test]
        public void Equality()
        {
            PropertyMutator<int> intMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");

            PropertyMutator<int> otherIntMutator = new PropertyMutator<int>(input => input * 2,
                input => input / 2, "SimplePropertyMutator");

            LambdaUtils.Eq<PropertyMutator<int>.Mutator>(intMutator.Mutate, otherIntMutator.Mutate);
            Assert.AreEqual(intMutator, otherIntMutator);
            Assert.AreEqual(otherIntMutator, intMutator);

            // Make sure counts don't interfere with equality
            otherIntMutator.Count = 500;
            Assert.AreEqual(intMutator, otherIntMutator);
            Assert.AreEqual(otherIntMutator, intMutator);
        }
    }
}