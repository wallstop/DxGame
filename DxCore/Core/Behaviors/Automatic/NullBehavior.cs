using System;
using System.Collections.Generic;
using DXGame.Core.Behaviors.Goals;
using System.Linq;
using DXGame.Core.Components.Advanced.Behaviors;
using DxCore.Core.Behaviors.Goals;
using NLog;

namespace DXGame.Core.Behaviors.Automatic
{
    /// <summary>
    /// The 'no-op' behavior, which is always satisfied and provides no goals.
    /// </summary>
    public class NullBehavior : Behavior
    {
        Logger Logger = LogManager.GetCurrentClassLogger();

        public Score FitnessFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            return Score.Min;
        }

        public bool SatisfiedFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            return true;
        }

        public Goal ResolveGoalFor(BehaviorComponent behaver)
        {
            Logger.Info($"Assigning {this.GetType().Name} to {nameof(behaver)}.");
            return new DoNothingGoal();
        }

        public override string ToString()
        {
            return "NullBehavior";
        }
    }
}
