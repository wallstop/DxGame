using DxCore.Core.Behaviors.Goals;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DXGame.Core.Behaviors.Goals;
using NLog;

namespace DxCore.Core.Components.Advanced.Behaviors
{
    // TODO: 
    /** 
     * This class logic can be almost entirely static, but it will need 
     * 1) to be populated with unique attribute data 
     * 2) custom but hopefully reuseable logic for "map goal of type X to commandment chain Y" 
     * 
     * Abstract class will break the Builder pattern; maybe expand the constructor to include an 'attribute map" and a "goal resolver"? 
     */

    public class BehaviorComponent : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // TODO Swanky tags required to support networked play?  Better ask Eli
        public Goal CurrentGoal { get; private set; }

        public BehaviorComponent()
        {
            // Behavior components start with very low expectations
            CurrentGoal = new DoNothingGoal();
        }

        public void AssignGoal(Goal goal)
        {
            Logger.Info($"{GetType().Name} assigned goal: {goal}");
            // No checks here: BehaviorComponents don't know when they should get new goals, they have to take our word for it
            CurrentGoal = goal;
        }

        /// <summary>
        /// Update and inspect the currently assigned goal, generating and emitting any messages required to support it
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(DxGameTime gameTime)
        {
            CurrentGoal.Update(gameTime, this);
        }
    }
}