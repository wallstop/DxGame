using System.Collections.Generic;
using DXGame.Core.Utils;
using log4net;

namespace DXGame.Core.State
{
    public class State
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (State));
        public ICollection<Transition> Transitions { get; }
        public string Name { get; }
        public Action Action { get; }
        public Presentation Presentation { get; }

        private State(ICollection<Transition> transitions, string name, Action action, Presentation presentation)
        {
            Validate.IsNotNullOrDefault(name, $"Cannot create a {nameof(State)} with a null name");
            Name = name;
            Transitions = new HashSet<Transition>();
            Action = action;
            Presentation = presentation;
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
            return Name.GetHashCode() ^ LambdaUtils.DelegateHashCode(Action);
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
            private Presentation presentation_;

            public State Build()
            {
                Validate.IsNotNullOrDefault(action_,
                    $"Cannot create a {nameof(State)} with a null/default {nameof(action_)}");
                Validate.IsNotNullOrDefault(presentation_,
                    $"Cannot create a {nameof(State)} with a null/default) {nameof(presentation_)}");
                Validate.IsNotNullOrDefault(name_,
                    $"Cannot create a {nameof(State)} with a null/default/empty {nameof(name_)}");
                if (transitions_.Count == 0)
                {
                    LOG.Info($"Creating {nameof(State)} ({name_}) without any transitions");
                }

                return new State(transitions_, name_, action_, presentation_);
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

            public StateBuilder WithPresentation(Presentation presentation)
            {
                Validate.IsNull(presentation_,
                    $"Cannot assign a {nameof(presentation)} to a Builder with an already assigned {nameof(presentation)}");
                presentation_ = presentation;
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