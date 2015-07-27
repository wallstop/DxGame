using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Behavior
{
    public class Behavior : Component
    {
        public State InitialState { get; }
        public State CurrentState { get; private set; }
        // TODO: Verify this is correct
        public int States
        {
            get
            {
                /* Breadth-first state graph traversal to determine number of States in state graph */
                Queue<State> statesToVisit = new Queue<State>();
                HashSet<State> seenStates = new HashSet<State>();
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

        public Behavior(DxGame game, State initialState) : base(game)
        {
            Validate.IsNotNull(game, $"Cannot create a {nameof(Behavior)} with a null{typeof (DxGame)}");
            Validate.IsNotNullOrDefault(initialState,
                $"Cannot create a {nameof(Behavior)} with a null {nameof(initialState)}");

            InitialState = initialState;
            Reset();
        }

        protected override void Update(DxGameTime gameTime)
        {
            CurrentState = CurrentState.Transitions.First(transition => transition.ShouldTransition(DxGame, gameTime))
                ?.State ?? CurrentState;
            CurrentState.Action(gameTime);
        }

        public void Reset()
        {
            CurrentState = InitialState;
        }

        public static BehaviorBuilder Builder()
        {
            return new BehaviorBuilder();
        }

        public class BehaviorBuilder
        {
            private DxGame game_;
            private State initialState_;

            public BehaviorBuilder WithDxGame(DxGame game)
            {
                game_ = game;
                return this;
            }

            public BehaviorBuilder WithInitialState(State initialState)
            {
                initialState_ = initialState;
                return this;
            }

            public Behavior Build()
            {
                /* Rely on validation inside Behavior to null-check inputs */
                return new Behavior(game_, initialState_);
            }
        }
    }
}