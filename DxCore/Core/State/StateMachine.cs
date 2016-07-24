using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using NLog;

namespace DxCore.Core.State
{
    [Serializable]
    [DataContract]
    public class StateMachine : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [DataMember]
        public State InitialState { get; private set; }

        [DataMember]
        public State CurrentState { get; private set; }

        [DataMember]
        private bool Logging { get; set; }

        [DataMember] private ReadOnlyDictionary<UniqueId, State> statesById_;

        protected StateMachine(State initialState, Dictionary<UniqueId, State> statesById, bool loggingEnabled)
        {
            Logging = loggingEnabled;
            UpdatePriority = UpdatePriority.State;
            InitialState = initialState;
            statesById_ = new ReadOnlyDictionary<UniqueId, State>(statesById);
            Reset();
        }

        public override void OnAttach()
        {
            RegisterTargetedAcceptAll(HandleMessage);
            base.OnAttach();
        }

        private void HandleMessage(Message message)
        {
            CurrentState.Accept(message);
        }

        protected override void Update(DxGameTime gameTime)
        {
            /*
                Find the first transition that has had it's trigger activated. 
            */
            State nextState;
            if(!CurrentState.Transition(out nextState))
            {
                nextState = CurrentState;
            }

            StateUpdateConfig updateConfig = new StateUpdateConfig(gameTime, Logging);

            /* Are we transitioning? Fire the enter and exit functions, boys! Land ho! */
            if(!ReferenceEquals(nextState, CurrentState))
            {
                if(Logging)
                {
                    Logger.Debug("Changing states from {0} to {1}", CurrentState, nextState);
                }
                CurrentState.Exit(updateConfig);
                nextState.Enter(updateConfig);
                CurrentState = nextState;
            }
            CurrentState.Process(updateConfig);
        }

        public void Reset()
        {
            CurrentState = InitialState;
        }

        public static StateMachineBuilder Builder()
        {
            return new StateMachineBuilder();
        }

        public class StateMachineBuilder : IBuilder<StateMachine>
        {
            private State initialState_;
            private bool logging_ = false;

            private readonly Dictionary<UniqueId, State> uniqueStates_ = new Dictionary<UniqueId, State>();

            public StateMachine Build()
            {
                Validate.Hard.IsNotNullOrDefault(initialState_, () => this.GetFormattedNullOrDefaultMessage(nameof(initialState_)));
                return new StateMachine(initialState_, uniqueStates_, logging_);
            }

            public StateMachineBuilder WithState(State state)
            {
                uniqueStates_[state.Id] = state;
                return this;
            }

            public StateMachineBuilder WithLogging(bool logging = true)
            {
                logging_ = logging;
                return this;
            }

            public StateMachineBuilder WithInitialState(State initialState)
            {
                initialState_ = initialState;
                WithState(initialState);
                return this;
            }
        }
    }
}