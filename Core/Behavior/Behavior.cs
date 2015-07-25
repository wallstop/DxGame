using System.Linq;
using DXGame.Core.Utils;

namespace DXGame.Core.Behavior
{
    public class Behavior
    {
        public State InitialState { get; }
        public State CurrentState { get; private set; }

        public Behavior(State initialState)
        {
            Validate.IsNotNull(initialState, "Cannot create a Behavior with a null initialState");
            InitialState = initialState;
            Reset();
        }

        public void Update()
        {
            CurrentState = CurrentState.Transitions.First(transition => transition.ShouldTransition())
                ?.State ?? CurrentState;
        }

        public void Reset()
        {
            CurrentState = InitialState;
        }
    }
}