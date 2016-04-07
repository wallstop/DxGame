using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;

namespace DXGame.Core.State
{
    [Serializable]
    [DataContract]
    public class StateMachine : Component
    {
        [DataMember]
        public State InitialState { get; private set; }

        [DataMember]
        public State CurrentState { get; private set; }

        protected StateMachine(State initialState)
        {
            InitialState = initialState;
            MessageHandler.EnableAcceptAll(HandleMessage);
            Reset();
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

            public StateMachine Build()
            {
                Validate.IsNotNullOrDefault(initialState_,
                    StringUtils.GetFormattedNullOrDefaultMessage(this, "InitialState"));
                return new StateMachine(initialState_);
            }

            public StateMachineBuilder WithInitialState(State initialState)
            {
                initialState_ = initialState;
                return this;
            }
        }
    }
}