
using DxCore.Core.Behaviors.Goals;
using DxCore.Core.Components.Advanced.Behaviors;
using DxCore.Core.Primitives;

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

        protected void MarkSuccessful()
        {
            Status = GoalStatus.Successful;
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
