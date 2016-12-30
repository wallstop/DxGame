using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DXGame.Core.Behaviors;
using DXGame.Core.Behaviors.Goals;
using DXGame.Core.Components.Advanced.Behaviors;
using NLog;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Behaviors
{
    /**
        The Sun Tzu of his time.

        <summary>
            Determines mappings of BehaviorComponent -> Goals
        </summary>
    */

    internal class GoalAssigner : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<BehaviorComponent, Behavior> assignments_ =
            new Dictionary<BehaviorComponent, Behavior>();

        private readonly IEnumerable<Behavior> candidateBehaviors_;

        public GoalAssigner(IEnumerable<Behavior> candidateBehaviors)
        {
            candidateBehaviors_ = candidateBehaviors;
        }

        /// <summary>
        /// Extract all BehaviorComponents and assign them the appropriate goals
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(DxGameTime gameTime)
        {
            AssignBehaviors();
        }

        /// <summary>
        /// Assign behaviors to entities
        /// </summary>
        private void AssignBehaviors()
        {
            // Extract all BehaviorComponents
            IEnumerable<BehaviorComponent> behaviorComponents =
                DxGame.Instance.DxGameElements.OfType<BehaviorComponent>().ToList();

            // For each component (presumably 1:1 with entity)...
            // TODO: Currently we only assign new goals when old goals have been met.  Implement more complex logic for 'interruptable' and 'non-interruptable' goals/behaviors
            foreach(BehaviorComponent behaver in behaviorComponents.Where(behaver => !behaver.CurrentGoal.InProgress()))
            {
                // Filter all behaviors by constraints...
                IEnumerable<Behavior> satisfiedBehaviors =
                    candidateBehaviors_.Where(behavior => behavior.SatisfiedFor(behaver, assignments_));
                Validate.Hard.IsNotEmpty(satisfiedBehaviors,
                    $"No behavior satisfied for {behaver}, not even the null behavior!  (This shouldn't ever happen.)");

                // Sort all remaining behaviors by fitness and take the most fit behavior...
                Behavior bestBehavior =
                    satisfiedBehaviors.OrderByDescending(behavior => behavior.FitnessFor(behaver, assignments_)).First();

                // Assign, purely to keep track of who is doing what...
                assignments_[behaver] = bestBehavior;

                // Apply situation-specific logic to acquire a goal, then assign the goal to a the entity's behavior component
                Goal goal = bestBehavior.ResolveGoalFor(behaver);
                Logger.Info($"Assigning {goal} to {behaver}.");
                behaver.AssignGoal(goal);
            }
        }
    }
}