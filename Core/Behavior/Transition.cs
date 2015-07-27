using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Behavior
{
    public class Transition
    {
        public Trigger Trigger { get; }
        public State State { get; }

        public Transition(Trigger trigger, State resultState)
        {
            Validate.IsNotNull(trigger, $"Cannot create a transition with a null {nameof(trigger)}");
            Validate.IsNotNull(resultState, $"Cannot create a Transition with a null {nameof(resultState)}");
            Trigger = trigger;
            State = resultState;
        }

        public bool ShouldTransition(DxGame game, DxGameTime gameTime)
        {
            return Trigger.Invoke(game, gameTime);
        }
    }
}