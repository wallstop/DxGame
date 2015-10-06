using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.DataStructures;
using DXGame.Core.Utils;
using NLog;

namespace DXGame.Core.State
{
    /**
        <summary>
            Encapsulates a single State that an enemy can be in, as part of a StateMachine.
            This is envisioned to be encapsulate movement states, attacking states, etc 
            (hopefully with distinct state machines for each!)
        </summary>
    */

    [Serializable]
    [DataContract]
    public sealed class State
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        public ICollection<Transition> Transitions { get; private set; }

        /* Simple descriptor of what the state is. "Jumping", "Moving Right", that kind of thing */

        [DataMember]
        public string Name { get; }

        /* Action that should be performed each and every Update cycle that this State is "current". Should never be null. */

        [DataMember]
        public Action Action { get; }

        /* Action that should only be performed once, on state entrance. Can be null. */

        [DataMember]
        public Action OnEnter { get; }

        /* Action that should only be performed once, on state exit. Can be null. */

        [DataMember]
        public Action OnExit { get; }

        private State(ICollection<Transition> transitions, string name, Action action, Action onEnter, Action onExit)
        {
            Name = name;
            Transitions = new SortedList<Transition>(transitions);
            Action = action;
            OnEnter = onEnter;
            OnExit = onExit;
        }

        public State WithTransition(Transition transition)
        {
            Validate.IsNotNull(transition, $"Cannot add a null {nameof(transition)} to a {nameof(State)}");
            Transitions.Add(transition);
            return this;
        }

        public static StateBuilder Builder()
        {
            return new StateBuilder();
        }

        public override bool Equals(object other)
        {
            var otherState = other as State;
            return otherState != null && Name.Equals(otherState.Name) &&
                   LambdaUtils.DelegateHashCode(Action).Equals(LambdaUtils.DelegateHashCode(otherState.Action));
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Name, LambdaUtils.DelegateHashCode(Action));
        }

        public override string ToString()
        {
            return Name;
        }

        public class StateBuilder : IBuilder<State>
        {
            private readonly List<Transition> transitions_ = new List<Transition>();
            private Action action_;
            private string name_;
            private Action onEnter_;
            private Action onExit_;

            public State Build()
            {
                Validate.IsNotNullOrDefault(action_,
                    $"Cannot create a {nameof(State)} with a null/default {nameof(action_)}");

                Validate.IsNotNullOrDefault(name_,
                    $"Cannot create a {nameof(State)} with a null/default/empty {nameof(name_)}");
                if (transitions_.Count == 0)
                {
                    LOG.Debug($"Creating {nameof(State)} ({name_}) without any transitions");
                }

                return new State(transitions_, name_, action_, onEnter_, onExit_);
            }

            public StateBuilder WithOnEnter(Action onEnter)
            {
                onEnter_ = onEnter;
                return this;
            }

            public StateBuilder WithOnExit(Action onExit)
            {
                onExit_ = onExit;
                return this;
            }

            public StateBuilder WithTransition(Transition transition)
            {
                transitions_.Add(transition);
                return this;
            }

            public StateBuilder WithAction(Action action)
            {
                Validate.IsNull(action_,
                    $"Cannot assign a {nameof(action)} to a Builder with an already assigned {nameof(action)}");
                action_ = action;
                return this;
            }

            public StateBuilder WithName(string name)
            {
                name_ = name;
                return this;
            }
        }
    }
}