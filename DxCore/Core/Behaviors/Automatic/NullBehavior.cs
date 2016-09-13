using System;
using System.Collections.Generic;
using DXGame.Core.Behaviors.Goals;
using System.Linq;
using DXGame.Core.Components.Advanced.Behaviors;
using DxCore.Core.Behaviors.Goals;

namespace DXGame.Core.Behaviors.Automatic
{
    /// <summary>
    /// The 'no-op' behavior, which is always satisfied and provides no goals.
    /// </summary>
    class NullBehavior : Behavior
    {
        public override Score GetFitnessFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            return Score.Min;
        }

        public override bool IsSatisfiedFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            return true;
        }

        public override Goal ResolveGoalFor(BehaviorComponent behaver)
        {
            Logger.Info($"Assigning {this.GetType().Name} to {nameof(behaver)}.");
            return new DoNothingGoal();
        }
    }
}
