using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NLog;

namespace DXGame.Core.Properties
{
    [Serializable]
    [DataContract]
    public sealed class Property<T>
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        // TODO: Figure out how to properly serialize / hide name
        [DataMember] public readonly string Name;

        [DataMember] private SortedDictionary<PropertyMutator<T>, int> mutatorCounts_ =
            new SortedDictionary<PropertyMutator<T>, int>(new PropertyMutatorPriorityComparer<T>());

        [DataMember]
        public T BaseValue { get; private set; }

        [DataMember]
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
                    $"Attempted to find a null {GetType()} from Property {Name}");
                return;
            }

            InternalAddMutator(mutator);
            if (LOG.IsDebugEnabled)
            {
                LOG.Debug(
                    $"Added {1} {GetType()} count for a total of {mutatorCounts_[mutator]} of PropertyMutator {mutator} to {Name}");
            }
        }

        public void RemoveMutator(PropertyMutator<T> mutator)
        {
            if (mutator == null || !mutatorCounts_.ContainsKey(mutator))
            {
                LOG.Error(
                    $"Attempted to remove non-existing {mutator} from Property {Name}");
                return;
            }

            if (LOG.IsDebugEnabled)
            {
                LOG.Debug(
                    $"Decremented Instance Count of PropertyMutator {mutator} from {mutatorCounts_[mutator]} to {mutatorCounts_[mutator] - 1} for Property {Name}");
            }

            InternalRemoveMutator(mutator);
        }

        /*
            Add a mutator to our collection, then re-order the collection based on priority. Assumes the mutator is non-null
        */

        private void InternalAddMutator(PropertyMutator<T> mutator)
        {
            ApplyDeMutatorChain();
            mutatorCounts_[mutator] = (mutatorCounts_.ContainsKey(mutator)
                ? mutatorCounts_[mutator] + 1
                : 1);
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