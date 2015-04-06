using System;
using DXGame.Core.Utils;

namespace DXGame.Core
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

        The Mutator function on a PropertyMutator
    </summary>
    */

    public abstract class PropertyMutator<T> : IEquatable<PropertyMutator<T>>
    {
        public delegate T DeMutator(T input);

        public delegate T Mutator(T input);

        protected readonly DeMutator deMutator_;
        protected readonly Mutator mutator_;
        public readonly string Name;
        public readonly MutatePriority Priority;
        public int Count;

        protected PropertyMutator(Mutator mutator, DeMutator demutator, string name)
        {
            // TODO: Remove these or do property validation checks
            GenericUtils.CheckNull(mutator);
            GenericUtils.CheckNull(demutator);
            GenericUtils.CheckNullOrDefault(name);
            Count = 1;
            mutator_ = mutator;
            deMutator_ = demutator;
            Name = name;
            Priority = MutatePriority.Medium;
        }

        public bool Equals(PropertyMutator<T> other) { return this == other; }

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
            return lhs != null && rhs != null
                   && lhs.deMutator_ == rhs.deMutator_
                   && lhs.mutator_ == rhs.mutator_
                   && lhs.Name == rhs.Name;
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
            return deMutator_.GetHashCode() ^ mutator_.GetHashCode() ^ Name.GetHashCode();
        }

        public override string ToString() { return Name; }

        public static int PriorityComparison(PropertyMutator<T> lhs, PropertyMutator<T> rhs)
        {
            return lhs.Priority.CompareTo(rhs.Priority);
        }
    }
}