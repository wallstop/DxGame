using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.State
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

        public Transition(Trigger trigger, State state)
            : this(trigger, state, Priority.Medium)
        {
        }

        public Transition(Trigger trigger, State state, Priority priority)
        {
            Validate.IsNotNull(trigger, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(trigger)));
            Validate.IsNotNull(state, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(state)));
            Validate.IsNotNull(priority, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(priority)));
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