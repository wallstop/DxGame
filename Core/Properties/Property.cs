using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace DXGame.Core.Properties
{
    public sealed class Property<T>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Property<T>));
        private readonly List<PropertyMutator<T>> mutators_ = new List<PropertyMutator<T>>();
        public readonly string Name;
        public T BaseValue { get; private set; }
        public T CurrentValue { get; set; }

        public Property(T value, string name)
        {
            BaseValue = value;
            CurrentValue = value;
            Name = name;
        }

        /*
            Note: Ignores mutator count. Only adds the mutator ONCE
        */

        public void AddMutator(PropertyMutator<T> mutator)
        {
            PropertyMutator<T> existingMutator = FindExistingMutator(mutator);
            // If it didn't exist, we need to add it
            if (existingMutator == null)
            {
                // Make sure its not null
                if (mutator == null)
                {
                    return;
                }
                InternalAddMutator(mutator);
                return;
            }

            LOG.Debug(String.Format(
                "Added {0} counts for a total of {1} of PropertyMutator {2} to {3}",
                mutator.Count,
                1, mutator.Name, Name));
            InternalUpdateMutator(existingMutator, 1);
        }

        public void RemoveMutator(PropertyMutator<T> mutator)
        {
            PropertyMutator<T> existingMutator = FindExistingMutator(mutator);
            if (existingMutator == null)
            {
                return;
            }

            // If we're here, that means that we have an existing PropertyMutator. Great!
            int existingCount = existingMutator.Count;
            if (existingCount <= 1)
            {
                InternalRemoveMutator(existingMutator);
                LOG.Debug(String.Format("Removed PropertyMutator {0} from Property {1}",
                    mutator.Name, Name));
            }
            else
            {
                InternalUpdateMutator(existingMutator, -1);
                LOG.Debug(
                    String.Format(
                        "Decremented Instance Count of PropertyMutator {0} from {1} to {2} for Property {3}",
                        existingMutator, existingCount, existingMutator.Count, Name));
            }
        }

        private PropertyMutator<T> FindExistingMutator(PropertyMutator<T> mutator)
        {
            if (mutator == null)
            {
                LOG.Error(
                    String.Format("Attempted to find a null PropertyMutator from Property {0}",
                        Name));
                return null;
            }

            PropertyMutator<T> existingMutator = mutators_.Find(item => item == mutator);
            if (existingMutator != default(PropertyMutator<T>))
            {
                return existingMutator;
            }
            LOG.Error(
                String.Format(
                    "Could not find existing PropertyMutator {0} for Property {1}",
                    mutator.Name, Name));
            return null;
        }

        /*
            Updates the count of an existing mutator. Assumes the mutator is a reference to one in the collection.
        */

        private void InternalUpdateMutator(PropertyMutator<T> mutator, int count)
        {
            ApplyDeMutatorChain();
            mutator.Count += count;
            ApplyMutatorChain();
        }

        /*
            Add a mutator to our collection, then re-order the collection based on priority. Assumes the mutator is non-null
        */

        private void InternalAddMutator(PropertyMutator<T> mutator)
        {
            ApplyDeMutatorChain();
            mutators_.Add(mutator);
            mutators_.Sort(PropertyMutator<T>.PriorityComparison);
            ApplyMutatorChain();
        }

        /*
            Remove a mutator from our collection. Assumes the collection is already in-order; thus removing an element 
            will not change the sorted-ness of it.

            Assumes the mutator currently exists in the collection.
        */

        private void InternalRemoveMutator(PropertyMutator<T> mutator)
        {
            ApplyDeMutatorChain();
            mutators_.Remove(mutator);
            ApplyMutatorChain();
        }

        private void ApplyMutatorChain()
        {
            CurrentValue = mutators_.Aggregate(BaseValue,
                (current, mutator) => mutator.Mutate(current));
        }

        private void ApplyDeMutatorChain()
        {
            BaseValue = mutators_.Aggregate(CurrentValue,
                (current, mutator) => mutator.DeMutate(current));
        }
    }
}