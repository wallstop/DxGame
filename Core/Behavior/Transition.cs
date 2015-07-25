using DXGame.Core.Utils;

namespace DXGame.Core.Behavior
{
    public class Transition
    {
        public Trigger Trigger { get; }
        public State State { get; }

        public Transition(Trigger trigger, State resultState)
        {
            Validate.IsNotNull(trigger, "Cannot create a transition with a null trigger");
            Validate.IsNotNull(resultState, "Cannot create a Transition with a null resultState");
            Trigger = trigger;
            State = resultState;
        }

        public bool ShouldTransition()
        {
            return Trigger.Invoke();
        }
    }
}