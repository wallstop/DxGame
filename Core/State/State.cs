using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.DataStructures;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using NLog;

namespace DXGame.Core.State
{
    [Serializable]
    [DataContract]
    public class State
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [DataMember]
        public ICollection<Transition> Transitions { get; private set; }

        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public Action Action { get; private set; }

        private State(ICollection<Transition> transitions, string name, Action action)
        {
            Validate.IsNotNullOrDefault(name, $"Cannot create a {nameof(State)} with a null name");
            Name = name;
            Transitions = new SortedList<Transition>(transitions);
            Action = action;
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

                return new State(transitions_, name_, action_);
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