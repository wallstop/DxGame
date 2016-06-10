using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using NLog;

namespace DxCore.Core.Properties
{
    public interface IProperty
    {
        void TriggerListeners();
    }

    /**
        <summary> 
            "Listens" to changes in a property, possibly reacting if so.
            A simple example would be to check if Health has dropped below 0 (and do something, like die)
        </summary>

        Example:
        <code>
            private void DeathListener(int previousHealth, int currentHealth)
            {
                if(currentHealth <= 0 && previousHealth > 0) 
                {
                    Console.WriteLine("Health has reached 0! Oh no!");
                    Environment.Exit(Int.MaxValue); // We simply cannot continue, the end is neigh
                }
            }
        </code>
    */

    public delegate void PropertyListener<in T>(T previous, T current);

    /**
        <summary>
            Represents a Property on an entity. This is typically imagined to be something along the lines of Health, Runspeed, Damage, Defense, etc.

            This is a nice encapsulation around the concept of "PropertyMutators", or modifiers. 
            This class handles the logic of adding & removing "buffs", so values will be able to change fluently with the addition & subtraction of buff-based values
        </summary>
    */

    [Serializable]
    [DataContract]
    public sealed class Property<T> : IProperty
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        // TODO: Convert these to weak references (Similar to EventModel?)
        [DataMember] private readonly List<PropertyListener<T>> listeners_ = new List<PropertyListener<T>>();

        [DataMember] private readonly SortedDictionary<PropertyMutator<T>, int> mutatorCounts_ =
            new SortedDictionary<PropertyMutator<T>, int>(new PropertyMutatorPriorityComparer<T>());

        [DataMember] private T baseValue_;
        [DataMember] private T currentValue_;
        
        [DataMember]
        public string Name { get; }

        [DataMember]
        public T BaseValue
        {
            get { return baseValue_; }
            set
            {
                baseValue_ = value;
                ApplyMutatorChain();
            }
        }

        [DataMember]
        public T CurrentValue => currentValue_;

        public Property(T value, string name)
        {
            BaseValue = value;
            Name = name;
        }

        public Property(Property<T> copy)
        {
            Validate.IsNotNull(copy, StringUtils.GetFormattedNullOrDefaultMessage(this, copy));
            BaseValue = copy.BaseValue;
            Name = copy.Name;
            listeners_.AddRange(copy.listeners_);
            foreach(var entry in copy.mutatorCounts_)
            {
                mutatorCounts_[entry.Key] = entry.Value;
            }
        }

        /* Manually trigger event listeners. This is useful for things that are "sleeping" when a condition happens */

        public void TriggerListeners()
        {
            T previous = CurrentValue;
            T current = CurrentValue;
            InternalTriggerListeners(previous, current);
        }

        private void InternalTriggerListeners(T previous, T current)
        {
            foreach(var listener in listeners_)
            {
                listener.Invoke(previous, current);
            }
        }

        public void AttachListener(PropertyListener<T> listener)
        {
            Validate.IsNotNullOrDefault(listener,
                $"Cannot attach a null {typeof(PropertyListener<T>)} to a {typeof(Property<T>)} ({Name})");
            listeners_.Add(listener);
        }

        public bool RemoveListener(PropertyListener<T> listener)
        {
            return listeners_.Remove(listener);
        }

        public void AddMutator(PropertyMutator<T> mutator)
        {
            if(mutator == null)
            {
                LOG.Error($"Attempted to find a null {GetType()} from Property {Name}");
                return;
            }

            InternalAddMutator(mutator);
            if(LOG.IsDebugEnabled)
            {
                LOG.Debug(
                    $"Added {1} {GetType()} count for a total of {mutatorCounts_[mutator]} of PropertyMutator {mutator} to {Name}");
            }
        }

        public void RemoveMutator(PropertyMutator<T> mutator)
        {
            if(mutator == null || !mutatorCounts_.ContainsKey(mutator))
            {
                LOG.Error($"Attempted to remove non-existing {mutator} from Property {Name}");
                return;
            }

            if(LOG.IsDebugEnabled)
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
            mutatorCounts_[mutator] = mutatorCounts_.ContainsKey(mutator) ? mutatorCounts_[mutator] + 1 : 1;
            ApplyMutatorChain();
        }

        /*
            Remove a mutator from our collection. Assumes the collection is already in-order; thus removing an element 
            will not change the sorted-ness of it.

            Assumes the mutator currently exists in the collection.
        */

        private void InternalRemoveMutator(PropertyMutator<T> mutator)
        {
            if(mutatorCounts_[mutator] > 1)
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
            T previous = currentValue_;
            currentValue_ = mutatorCounts_.Aggregate(BaseValue,
                (current, mutatorCountPair) => mutatorCountPair.Key.Mutate(current, mutatorCountPair.Value));
            InternalTriggerListeners(previous, currentValue_);
        }
    }
}