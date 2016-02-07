using DXGame.Core.Properties;
using NUnit.Framework;

namespace DXGameTest.Core.Property
{
    public class PropertyMutatationChain
    {
        [Test]
        public void NormalBaseAndCurrentValueGeneration()
        {
            const int baseValue = 300;
            const string propertyName = "TestIntegerProperty";
            Property<int> property = new Property<int>(baseValue, propertyName);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            // Make sure a simple pass-through mutator has no effect on the data
            var mutator = new PropertyMutator<int>((input, count) => input, "NoOpMutator");
            property.AddMutator(mutator);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            // And that removing it works
            property.RemoveMutator(mutator);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            // Make sure a simple + 1 mutator properly effects the state
            mutator = new PropertyMutator<int>((input, count) => input + 1, "NoOpMutator");

            property.AddMutator(mutator);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue + 1, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            // And that removing it actually removes it and generates the right value
            property.RemoveMutator(mutator);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);
        }

        [Test]
        public void CurrentValueChanges()
        {
            const int baseValue = 300;
            const string propertyName = "TestIntegerProperty";
            Property<int> property = new Property<int>(baseValue, propertyName);

            var mutator = new PropertyMutator<int>((input, count) => input + 1, "NoOpMutator");

            property.AddMutator(mutator);

            // Make sure the mutator works properly (should just be adding 1 to current value)
            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue + 1, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            const int removedAmount = 2;
            property.BaseValue -= removedAmount;

            // We expect that if we modify the CurrentValue, only the currentValue will change
            Assert.AreEqual(baseValue - removedAmount, property.BaseValue);
            Assert.AreEqual(baseValue + 1 - removedAmount, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            // However, we also expect that this change will carry backwards once the mutator is removed and effect both the base & current values
            property.RemoveMutator(mutator);

            Assert.AreEqual(baseValue - removedAmount, property.BaseValue);
            Assert.AreEqual(baseValue - removedAmount, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);
        }

        [Test]
        public void AddAndRemoveMultipleCountsSameMutator()
        {
            const int baseValue = 300;
            const string propertyName = "TestIntegerProperty";
            Property<int> property = new Property<int>(baseValue, propertyName);

            var mutator = new PropertyMutator<int>((input, count) => input + 1, "NoOpMutator");

            const int numMutators = 1000;
            for(int i = 0; i < numMutators; ++i)
            {
                property.AddMutator(mutator);
            }

            // Make sure the mutators haven't foobard the property's state
            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue + numMutators, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            const int partialRemove = 100;
            for(int i = 0; i < partialRemove; ++i)
            {
                property.RemoveMutator(mutator);
            }

            // Make sure the mutators haven't foobard the property's state
            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue + numMutators - partialRemove, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            const int modificationAmount = 35;
            property.BaseValue -= modificationAmount;

            Assert.AreEqual(baseValue - modificationAmount, property.BaseValue);
            Assert.AreEqual(baseValue + numMutators - partialRemove - modificationAmount, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            for(int i = 0; i < numMutators - partialRemove; ++i)
            {
                property.RemoveMutator(mutator);
            }
            Assert.AreEqual(baseValue - modificationAmount, property.BaseValue);
            Assert.AreEqual(
                baseValue + numMutators - partialRemove - (numMutators - partialRemove) - modificationAmount,
                property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);
        }

        [Test]
        public void AddNullMutatorNoOp()
        {
            const int baseValue = 300;
            const string propertyName = "TestIntegerProperty";
            Property<int> property = new Property<int>(baseValue, propertyName);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            property.AddMutator(null);

            // If we had added it, we should have gotten a null reference exception here
            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);
        }

        [Test]
        public void RemoevNonExistantMutator()
        {
            const int baseValue = 300;
            const string propertyName = "TestIntegerProperty";
            Property<int> property = new Property<int>(baseValue, propertyName);

            // Just check to see if no exception is thrown
            property.RemoveMutator(null);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);

            var mutator = new PropertyMutator<int>((input, count) => input + 1, "NoOpMutator");

            // Just check to see if no exception is thrown
            property.RemoveMutator(mutator);

            Assert.AreEqual(baseValue, property.BaseValue);
            Assert.AreEqual(baseValue, property.CurrentValue);
            Assert.AreEqual(propertyName, property.Name);
        }
    }
}