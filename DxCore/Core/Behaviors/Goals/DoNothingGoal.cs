using DXGame.Core.Behaviors.Goals;
using System;
using DxCore.Core.Components.Advanced.Behaviors;
using DxCore.Core.Primitives;
using NLog;

namespace DxCore.Core.Behaviors.Goals
{
    public class DoNothingGoal : Goal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DoNothingGoal()
        {
            // You did it!  Immediately complete this goal so it can be replaced
            Status = GoalStatus.Successful;
        }

        public override void Update(DxGameTime gameTime, BehaviorComponent behaver)
        {
            // Pass; this goal does nothing
        }
    }
}
