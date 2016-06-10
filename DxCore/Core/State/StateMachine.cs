using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.State
{
    [Serializable]
    [DataContract]
    public class StateMachine : Component
    {
        [DataMember]
        public State InitialState { get; private set; }

        [DataMember]
        public State CurrentState { get; private set; }

        [DataMember]
        private ReadOnlyDictionary<UniqueId, State> statesById_; 

        protected StateMachine(State initialState, Dictionary<UniqueId, State> statesById)
        {
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

            /* Are we transitioning? Fire the enter and exit functions, boys! Land ho! */
            if(!ReferenceEquals(nextState, CurrentState))
            {
                CurrentState.Exit(gameTime);
                nextState.Enter(gameTime);
                CurrentState = nextState;
            }
            CurrentState.Process(gameTime);
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

            private readonly Dictionary<UniqueId, State> uniqueStates_ = new Dictionary<UniqueId, State>(); 

            public StateMachine Build()
            {
                Validate.IsNotNullOrDefault(initialState_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "InitialState"));
                return new StateMachine(initialState_, uniqueStates_);
            }

            public StateMachineBuilder WithState(State state)
            {
                uniqueStates_[state.Id] = state;
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