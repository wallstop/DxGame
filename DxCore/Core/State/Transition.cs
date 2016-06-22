using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.State
{
    public enum Priority
    {
        High = -1,
        Medium = 0,
        Low = 1
    }

    public delegate bool Trigger(List<Message> messages, DxGameTime gameTime);

    [Serializable]
    [DataContract]
    public class Transition : IComparable<Transition>
    {
        [DataMember]
        public Priority Priority { get; private set; }

        [DataMember]
        public Trigger Trigger { get; private set; }

        [DataMember]
        public State State { get; private set; }

        public Transition(Trigger trigger, State state) : this(trigger, state, Priority.Medium) {}

        public Transition(Trigger trigger, State state, Priority priority)
        {
            Validate.Hard.IsNotNull(trigger, this.GetFormattedNullOrDefaultMessage(nameof(trigger)));
            Validate.Hard.IsNotNull(state, this.GetFormattedNullOrDefaultMessage(nameof(state)));
            Validate.Hard.IsNotNull(priority, this.GetFormattedNullOrDefaultMessage(nameof(priority)));
            Trigger = trigger;
            Priority = priority;
            State = state;
        }

        public int CompareTo(Transition other)
        {
            return Priority.CompareTo(other?.Priority);
        }
    }
}