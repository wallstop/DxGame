using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;

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

        // TODO: Verify this is correct
        [IgnoreDataMember]
        public int States
        {
            get
            {
                /* Breadth-first state graph traversal to determine number of States in state graph */
                var statesToVisit = new Queue<State>();
                var seenStates = new HashSet<State>();
                statesToVisit.Enqueue(InitialState);

                do
                {
                    var currentState = statesToVisit.Dequeue();
                    foreach (var transition in currentState.Transitions)
                    {
                        var state = transition.State;
                        if (!seenStates.Add(state))
                        {
                            statesToVisit.Enqueue(transition.State);
                        }
                    }
                } while (statesToVisit.Any());
                return seenStates.Count;
            }
        }

        protected StateMachine(State initialState)
        {
            InitialState = initialState;
            Reset();
        }

        protected override void Update(DxGameTime gameTime)
        {
            /*
                Find the first transition that has had it's trigger activated. 
            */
            var nextState = CurrentState.Transitions.FirstOrDefault(
                transition => transition.ShouldTransition(Parent, gameTime))
                ?.State ?? CurrentState;
            /* Are we transitioning? Fire the enter and exit functions, boys! Land ho! */
            if (!ReferenceEquals(nextState, CurrentState))
            {

                CurrentState.OnExit?.Invoke(gameTime);
                nextState.OnEnter?.Invoke(gameTime);
                CurrentState = nextState;
            }
            CurrentState.Action(gameTime);
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