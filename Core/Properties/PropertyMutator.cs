using System;
using DXGame.Core.Utils;

namespace DXGame.Core.Properties
{
    /*
        The order in which PropertyMutators are applied, Highest first.
    */

    public enum MutatePriority
    {
        High,
        Medium,
        Low
    }

    /**
    <summary>
        PropertyMutators are small wrappers around two-way functions that interact with Properties.
        Properties have a BaseValue and a CurrentValue. How they get from one to another is via PropertyMutators. 
        Going from Base -> Current is done via Mutate() calls, going from Current -> Base is done via DeMutate() calls.

        Essentially, PropertyMutators are buffs/debuffs on certain properties. For example, a PropertyMutator that gives an attack speed buff could be defined as follows:
    
        <code>
        public class WereWolfBuff : PropertyMutator<float>
        {
            // Increases the property by 15%
            private readonly float BUFF_INCREASE = 1.15f;

            public WearWolfBuff()
                : base(WereWolfBuff, WereWolfDebuff, "WereWolfBuff")
            {
            }

            private static float WereWolfBuff(float input)
            {
                return input * BUFF_INCREASE;
            }

            private static float WereWolfDebuff(float input)
            {
                return input / BUFF_INCREASE;
            }
        }
        </code>

        The Mutator function on a PropertyMutator reflects the effect being applied (mutates the original value into some new value).

        The DeMutator function on a PropertyMutator reflects the effect bein un-applied (de-mutates the mutated value to derive the original value). 
        This is necessary because everything interacts with the mutated value of a Property (the current value). However, PropertyMutators interact
        with the base value to derive the current value, so we need to work backwards as well.

        NOTE: PropertyMutators currently cannot properly cope with lambda mutators & demutators. Te equality comparisons will fail.
    </summary>
    */

    public class PropertyMutator<T> : IEquatable<PropertyMutator<T>>
    {
        // TODO: Come up with a way to handle equality comparisons for lambda expressions
        public delegate T DeMutator(T input);

        public delegate T Mutator(T input);

        protected readonly DeMutator deMutator_;
        protected readonly Mutator mutator_;
        public readonly string Name;
        public readonly MutatePriority Priority;
        public int Count;

        public PropertyMutator(Mutator mutator, DeMutator demutator, string name, MutatePriority priority = MutatePriority.Medium)
        {
            // TODO: Remove these or do property validation checks
            GenericUtils.CheckNull(mutator);
            GenericUtils.CheckNull(demutator);
            GenericUtils.CheckNullOrDefault(name);
            Count = 1;
            mutator_ = mutator;
            deMutator_ = demutator;
            Name = name;
            Priority = priority;
        }

        public bool Equals(PropertyMutator<T> other)
        {
            return !ReferenceEquals(other, null) &&
                   deMutator_ == other.deMutator_ &&
                   mutator_ == other.mutator_ &&
                   Name == other.Name &&
                   Priority == other.Priority;
        }

        /*
            Default behavior is to Apply the Mutate & DeMutate functions $Count times
        */

        public virtual T DeMutate(T input)
        {
            for (int i = 0; i < Count; ++i)
            {
                input = deMutator_(input);
            }
            return input;
        }

        public virtual T Mutate(T input)
        {
            for (int i = 0; i < Count; ++i)
            {
                input = mutator_(input);
            }
            return input;
        }

        public static bool operator ==(PropertyMutator<T> lhs, PropertyMutator<T> rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(PropertyMutator<T> lhs, PropertyMutator<T> rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(Object other)
        {
            var lhs = other as PropertyMutator<T>;
            return lhs != null && Equals(lhs);
        }

        public override int GetHashCode()
        {
            return LambdaUtils.DelegateHashCode(deMutator_) ^ LambdaUtils.DelegateHashCode(mutator_) ^
                   Name.GetHashCode() ^ Priority.GetHashCode();
        }

        public override string ToString() { return Name; }

        public static int PriorityComparison(PropertyMutator<T> lhs, PropertyMutator<T> rhs)
        {
            return lhs.Priority.CompareTo(rhs.Priority);
        }
    }
}