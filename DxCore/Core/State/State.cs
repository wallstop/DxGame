using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.DataStructures;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.State
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
    public sealed class State : IIdentifiable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember] private readonly SortedList<Transition> transitions_;

        [DataMember] private UniqueId id_;

        [DataMember] private List<Message> messageBuffer_;

        [IgnoreDataMember] private State transition_;

        [DataMember] private bool triggered_;

        /* Simple descriptor of what the state is. "Jumping", "Moving Right", that kind of thing */

        [DataMember]
        public string Name { get; }

        [IgnoreDataMember]
        public IEnumerable<Transition> Transitions => transitions_;

        public UpdatePriority UpdatePriority => UpdatePriority.Normal;

        /* Action that should be performed each and every Update cycle that this State is "current". Should never be null. */

        [DataMember]
        private Action<List<Message>, DxGameTime> Action { get; }

        /* Action that should only be performed once, on state entrance. Can be null. */

        [DataMember]
        private Action<StateUpdateConfig> OnEnter { get; }

        /* Action that should only be performed once, on state exit. Can be null. */

        [DataMember]
        private Action<StateUpdateConfig> OnExit { get; }

        private State(ICollection<Transition> transitions, string name, Action<List<Message>, DxGameTime> action,
            Action<StateUpdateConfig> onEnter, Action<StateUpdateConfig> onExit)
        {
            Name = name;
            messageBuffer_ = new List<Message>();
            transitions_ = new SortedList<Transition>(transitions);
            Action = action;
            OnEnter = onEnter;
            OnExit = onExit;
            id_ = new UniqueId();
        }

        public UniqueId Id => id_;

        public void Accept(Message message)
        {
            messageBuffer_.Add(message);
        }

        public static StateBuilder Builder()
        {
            return new StateBuilder();
        }

        public int CompareTo(IProcessable other)
        {
            throw new NotImplementedException();
        }

        public void Enter(StateUpdateConfig updateConfig)
        {
            Reset();
            OnEnter?.Invoke(updateConfig);
        }

        public override bool Equals(object other)
        {
            var otherState = other as State;
            return (otherState != null) && Name.Equals(otherState.Name) &&
                   LambdaUtils.DelegateHashCode(Action).Equals(LambdaUtils.DelegateHashCode(otherState.Action));
        }

        public void Exit(StateUpdateConfig updateConfig)
        {
            Reset();
            OnExit?.Invoke(updateConfig);
        }

        public override int GetHashCode()
        {
            return Objects.HashCode(Name, LambdaUtils.DelegateHashCode(Action));
        }

        public void Process(StateUpdateConfig updateConfig)
        {
            List<Message> lastFrameMessages = SwapBuffers();
            if(updateConfig.LoggingEnabled)
            {
                Logger.Debug("Processing {0} with {1}", Name, string.Join(",", lastFrameMessages.Select(_ =>
                {
                    try
                    {
                        return _.ToJson();
                    }
                    catch
                    {
                        return _.ToString();
                    }
                }).ToArray()));
            }

            PrepTransition(lastFrameMessages, updateConfig.GameTime);
            if(ReferenceEquals(transition_, null) || ReferenceEquals(transition_, this))
            {
                Action.Invoke(lastFrameMessages, updateConfig.GameTime);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Transition(out State nextState)
        {
            nextState = transition_;
            return !ReferenceEquals(transition_, null);
        }

        public State WithTransition(Transition transition)
        {
            Validate.Hard.IsNotNull(transition, () => $"Cannot add a null {nameof(transition)} to a {nameof(State)}");
            transitions_.Add(transition);
            return this;
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

        private List<Message> SwapBuffers()
        {
            List<Message> oldMessages = messageBuffer_;
            messageBuffer_ = new List<Message>();
            return oldMessages;
        }

        public class StateBuilder : IBuilder<State>
        {
            private readonly List<Transition> transitions_ = new List<Transition>();
            private Action<List<Message>, DxGameTime> action_;
            private string name_;
            private Action<StateUpdateConfig> onEnter_;
            private Action<StateUpdateConfig> onExit_;

            public State Build()
            {
                Validate.Hard.IsNotNullOrDefault(action_,
                    () => $"Cannot create a {nameof(State)} with a null/default {nameof(action_)}");

                Validate.Hard.IsNotNullOrDefault(name_,
                    () => $"Cannot create a {nameof(State)} with a null/default/empty {nameof(name_)}");
                if(transitions_.Count == 0)
                {
                    Logger.Trace($"Creating {nameof(State)} ({name_}) without any transitions");
                }
                Validate.Hard.NoNullElements(transitions_,
                    () => $"Cannot create a {nameof(State)} with null transitions");

                return new State(transitions_, name_, action_, onEnter_, onExit_);
            }

            public StateBuilder WithAction(Action<List<Message>, DxGameTime> action)
            {
                Validate.Hard.IsNull(action_,
                    () => $"Cannot assign a {nameof(action)} to a Builder with an already assigned {nameof(action)}");
                action_ = action;
                return this;
            }

            public StateBuilder WithEntrance(Action<StateUpdateConfig> onEnter)
            {
                onEnter_ = onEnter;
                return this;
            }

            public StateBuilder WithExit(Action<StateUpdateConfig> onExit)
            {
                onExit_ = onExit;
                return this;
            }

            public StateBuilder WithName(string name)
            {
                name_ = name;
                return this;
            }

            public StateBuilder WithTransition(Transition transition)
            {
                transitions_.Add(transition);
                return this;
            }
        }
    }
}