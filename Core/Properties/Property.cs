using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace DXGame.Core.Properties
{
    public sealed class Property<T>
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Property<T>));

        private readonly SortedDictionary<PropertyMutator<T>, int> mutatorCounts_ =
            new SortedDictionary<PropertyMutator<T>, int>(new PropertyMutatorPriorityComparer<T>());

        public readonly string Name;
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public T BaseValue { get; private set; }
        public T CurrentValue { get; set; }

        public Property(T value, string name)
        {
            BaseValue = value;
            CurrentValue = value;
            Name = name;
        }

        public void AddMutator(PropertyMutator<T> mutator)
        {
            if (mutator == null)
            {
                LOG.Error(
                    String.Format("Attempted to find a null PropertyMutator from Property {0}",
                        Name));
                return;
            }

            InternalAddMutator(mutator);
            LOG.Debug(String.Format(
                "Added {0} PropertyMutator count for a total of {1} of PropertyMutator {2} to {3}",
                1, mutatorCounts_[mutator], mutator, Name));
        }

        public void RemoveMutator(PropertyMutator<T> mutator)
        {
            if (mutator == null || !mutatorCounts_.ContainsKey(mutator))
            {
                LOG.Error(
                    String.Format(
                        "Attempted to remove non-existing PropertyMutator {0} from Property {1}",
                        mutator, Name));
                return;
            }

            LOG.Debug(
                String.Format(
                    "Decremented Instance Count of PropertyMutator {0} from {1} to {2} for Property {3}",
                    mutator, mutatorCounts_[mutator], mutatorCounts_[mutator] - 1, Name));

            InternalRemoveMutator(mutator);
        }

        /*
            Add a mutator to our collection, then re-order the collection based on priority. Assumes the mutator is non-null
        */

        private void InternalAddMutator(PropertyMutator<T> mutator)
        {
            ApplyDeMutatorChain();
            mutatorCounts_[mutator] = (mutatorCounts_.ContainsKey(mutator)
                ? mutatorCounts_[mutator] + 1 : 1);
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
            if (mutatorCounts_[mutator] > 1)
            {
                --mutatorCounts_[mutator];
            }
            else
            {
                mutatorCounts_.Remove(mutator);
            }
            ApplyMutatorChain();
        }

        private void ApplyMutatorChain()
        {
            CurrentValue = mutatorCounts_.Aggregate(BaseValue,
                (current, mutatorCountPair) =>
                    mutatorCountPair.Key.Mutate(current, mutatorCountPair.Value));
        }

        private void ApplyDeMutatorChain()
        {
            BaseValue = mutatorCounts_.Aggregate(CurrentValue,
                (current, mutatorCountPair) =>
                    mutatorCountPair.Key.DeMutate(current, mutatorCountPair.Value));
        }
    }
}