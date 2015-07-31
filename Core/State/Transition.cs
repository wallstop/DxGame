using System;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.State
{
    public enum Priority
    {
        HIGH = -1,
        MEDIUM = 0,
        LOW = 1
    }

    public class Transition : IComparable<Transition>
    {
        public Priority Priority { get; }
        public Trigger Trigger { get; }
        public State State { get; }

        public Transition(Trigger trigger, State resultState)
            : this(trigger, resultState, Priority.MEDIUM)
        {
        }

        public Transition(Trigger trigger, State resultState, Priority priority)
        {
            Validate.IsNotNull(trigger, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(trigger)));
            Validate.IsNotNull(resultState, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(resultState)));
            Validate.IsNotNull(priority, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(priority)));
            Trigger = trigger;
            State = resultState;
            Priority = priority;
        }

        public int CompareTo(Transition other)
        {
            return Priority.CompareTo(other?.Priority);
        }

        public bool ShouldTransition(DxGame game, DxGameTime gameTime)
        {
            return Trigger.Invoke(game, gameTime);
        }
    }
}