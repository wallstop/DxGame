
using DxCore.Core.Behaviors.Goals;
using DxCore.Core.Primitives;
using DXGame.Core.Components.Advanced.Behaviors;

namespace DXGame.Core.Behaviors.Goals
{
    /// <summary>
    ///  A 'Goal' represents a concrete objective an entity is currently working towards.
    ///  Goals can be pending, successful, or failed.
    /// </summary>
    public abstract class Goal
    {
        public GoalStatus Status { get; protected set; }

        public Goal()
        {
            Status = GoalStatus.InProgress;
        }

        public bool InProgress()
        {
            return Status == GoalStatus.InProgress;
        }

        /// <summary>
        /// Emit appropriate messages, update internal state and status, etc.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="behaver">Component associated with this instance of this Goal</param>
        // TODO: Is there a cleaner way to associate goals and their entities? 
        public abstract void Update(DxGameTime gameTime, BehaviorComponent behaver);
    }
}
