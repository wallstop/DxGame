using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.DataStructures;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
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
    public sealed class State : IProcessable
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        public UpdatePriority UpdatePriority => UpdatePriority.NORMAL;

        [DataMember] private bool triggered_ = false;

        [IgnoreDataMember] private State transition_;

        [DataMember] private readonly SortedList<Transition> transitions_;

        [DataMember] private List<Message> messageBuffer_;

        [IgnoreDataMember]
        public IEnumerable<Transition> Transitions => transitions_;

        /* Simple descriptor of what the state is. "Jumping", "Moving Right", that kind of thing */

        [DataMember]
        public string Name { get; }

        /* Action that should be performed each and every Update cycle that this State is "current". Should never be null. */

        [DataMember]
        private Action<List<Message>, DxGameTime> Action { get; }

        /* Action that should only be performed once, on state entrance. Can be null. */

        [DataMember]
        private Action<DxGameTime> OnEnter { get; }

        /* Action that should only be performed once, on state exit. Can be null. */

        [DataMember]
        private Action<DxGameTime> OnExit { get; }

        private State(ICollection<Transition> transitions, string name, Action<List<Message>, DxGameTime> action, Action<DxGameTime> onEnter, Action<DxGameTime> onExit)
        {
            Name = name;
            messageBuffer_ = new List<Message>();
            transitions_ = new SortedList<Transition>(transitions);
            Action = action;
            OnEnter = onEnter;
            OnExit = onExit;
        }

        public void Accept(Message message)
        {
            Validate.IsNotNullOrDefault(message, $"{typeof(State)} cannot process a null message");
            messageBuffer_.Add(message);
        }

        public State WithTransition(Transition transition)
        {
            Validate.IsNotNull(transition, $"Cannot add a null {nameof(transition)} to a {nameof(State)}");
            transitions_.Add(transition);
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

        public int CompareTo(IProcessable other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }

        public class StateBuilder : IBuilder<State>
        {
            private readonly List<Transition> transitions_ = new List<Transition>();
            private Action<List<Message>, DxGameTime> action_;
            private string name_;
            private Action<DxGameTime> onEnter_;
            private Action<DxGameTime> onExit_;

            public State Build()
            {
                Validate.IsNotNullOrDefault(action_,
                    $"Cannot create a {nameof(State)} with a null/default {nameof(action_)}");

                Validate.IsNotNullOrDefault(name_,
                    $"Cannot create a {nameof(State)} with a null/default/empty {nameof(name_)}");
                if(transitions_.Count == 0)
                {
                    LOG.Trace($"Creating {nameof(State)} ({name_}) without any transitions");
                }
                Validate.NoNullElements(transitions_, $"Cannot create a {nameof(State)} with null transitions");

                return new State(transitions_, name_, action_, onEnter_, onExit_);
            }

            public StateBuilder WithEntrance(Action<DxGameTime> onEnter)
            {
                onEnter_ = onEnter;
                return this;
            }

            public StateBuilder WithExit(Action<DxGameTime> onExit)
            {
                onExit_ = onExit;
                return this;
            }

            public StateBuilder WithTransition(Transition transition)
            {
                transitions_.Add(transition);
                return this;
            }

            public StateBuilder WithAction(Action<List<Message>, DxGameTime> action)
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

        public void Enter(DxGameTime gameTime)
        {
            Reset();
            OnEnter?.Invoke(gameTime);
        }

        public void Exit(DxGameTime gameTime)
        {
            Reset();
            OnExit?.Invoke(gameTime);
        }

        public bool Transition(out State nextState)
        {
            nextState = transition_;
            return !ReferenceEquals(transition_, null);
        }

        public void Process(DxGameTime gameTime)
        {
            List<Message> lastFrameMessages = SwapBuffers();
            PrepTransition(lastFrameMessages, gameTime);
            if(ReferenceEquals(transition_, null) || ReferenceEquals(transition_, this))
            {
                Action.Invoke(lastFrameMessages, gameTime);
            }
        }

        private List<Message> SwapBuffers()
        {
            List<Message> oldMessages = messageBuffer_;
            messageBuffer_ = new List<Message>();
            return oldMessages;
        }

        private void PrepTransition(List<Message> lastFrameMessages, DxGameTime gameTime)
        {
            /* If this is the first time we've entered, don't check for transitions */
            if(!triggered_)
            {
                triggered_ = true;
                return;
            }

            foreach(Transition possibleTransition in transitions_)
            {
                if(possibleTransition.Trigger(lastFrameMessages, gameTime))
                {
                    transition_ = possibleTransition.State;
                    return;
                }
            }
            transition_ = null;
        }

        private void Reset()
        {
            SwapBuffers();
            transition_ = null;
            triggered_ = false;
        }
    }
}