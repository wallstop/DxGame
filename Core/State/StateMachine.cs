using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.State
{
    public class StateMachine : Component
    {
        public State InitialState { get; }
        public State CurrentState { get; private set; }
        // TODO: Verify this is correct
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

        public StateMachine(DxGame game, State initialState) : base(game)
        {
            Validate.IsNotNullOrDefault(initialState, StringUtils.GetFormattedNullOrDefaultMessage(this, initialState));
            InitialState = initialState;
            Reset();
        }

        protected override void Update(DxGameTime gameTime)
        {
            CurrentState = CurrentState.Transitions.FirstOrDefault(
                transition => transition.ShouldTransition(DxGame, gameTime))
                ?.State ?? CurrentState;
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
            private DxGame game_;
            private State initialState_;

            public StateMachine Build()
            {
                /* Rely on validation inside StateMachine to null-check inputs */
                return new StateMachine(game_, initialState_);
            }

            public StateMachineBuilder WithDxGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public StateMachineBuilder WithInitialState(State initialState)
            {
                initialState_ = initialState;
                return this;
            }
        }
    }
}