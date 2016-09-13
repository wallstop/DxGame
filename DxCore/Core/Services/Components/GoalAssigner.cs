using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Behaviors;
using DXGame.Core.Behaviors.Goals;
using DXGame.Core.Components.Advanced.Behaviors;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace DxCore.Core.Components.Advanced.Behaviors
{
    /**
        The Sun Tzu of his time.

        <summary>
            Determines mappings of BehaviorComponent -> Goals
        </summary>
    */
    class GoalAssigner : Component
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<BehaviorComponent, Behavior> assignments_ = new Dictionary<BehaviorComponent, Behavior>();
        private readonly IEnumerable<Behavior> candidateBehaviors_;

        public GoalAssigner(IEnumerable<Behavior> candidateBehaviors) : base()
        {
            candidateBehaviors_ = candidateBehaviors;
        }

        /// <summary>
        /// Extract all BehaviorComponents and assign them the appropriate goals
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(DxGameTime gameTime)
        {
            CleanUpBehaviors();
            AssignBehaviors();
        }

        /// <summary>
        /// Purge behaviors from the current mapping in the event that they complete or enter an error state
        /// </summary>
        private void CleanUpBehaviors()
        {

        }

        /// <summary>
        /// Assign behaviors to entities
        /// </summary>
        private void AssignBehaviors()
        {
            // Extract all BehaviorComponents
            IEnumerable<BehaviorComponent> behaviorComponents = DxGame.Instance.DxGameElements.OfType<BehaviorComponent>().ToList();

            // For each component (presumably 1:1 with entity)...
            foreach (BehaviorComponent behaver in behaviorComponents)
            {
                // Filter all behaviors by constraints...
                IEnumerable<Behavior> satisfiedBehaviors = candidateBehaviors_.Where(behavior => behavior.IsSatisfiedFor(behaver, assignments_));
                Validate.Hard.IsNotEmpty(satisfiedBehaviors, $"No behavior satisfied for {behaver}, not even the null behavior!  (This shouldn't ever happen.)");

                // Sort all remaining behaviors by fitness and take the most fit behavior
                Behavior bestBehavior = satisfiedBehaviors.OrderByDescending(behavior => behavior.GetFitnessFor(behaver, assignments_)).First();

                // Assign
                assignments_.Add(behaver, bestBehavior);

                // Apply situation-specific logic to acquire a goal, and assign the goal to a the entity's behavior component
                Goal goal = bestBehavior.ResolveGoalFor(behaver);
                // TODO: Assign goals
                // behaver.AssignGoals(goal);
            }
        }
    }
}
