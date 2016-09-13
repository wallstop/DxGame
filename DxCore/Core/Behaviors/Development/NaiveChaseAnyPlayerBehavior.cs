using System;
using System.Collections.Generic;
using DXGame.Core.Behaviors.Goals;
using DXGame.Core.Components.Advanced.Behaviors;
using DxCore.Core.Utils;
using DxCore.Core.Messaging;
/*
namespace DXGame.Core.Behaviors.Development
{
    class NaiveChaseAnyPlayerBehavior : Behavior
    {
        public override Score GetFitnessFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            // Fitness scales directly with movement speed
            AffinityComponent affinityComponent = BehaviorUtils.GetAffinityComponent(behaver);
            //? 'Movement' vs. Move + cardinal directions? 
            Optional<Score> maybeMovementSpeedScore = affinityComponent.AffinityFor(Messaging.Commandment.Movement, Attribute.Speed);
            return maybeMovementSpeedScore.HasValue ? maybeMovementSpeedScore.Value : Score.Min;
        }

        /// <summary>
        /// The chasing behavior applies to all entities who can move, expressed as any non-zero score for Movement:Speed
        /// </summary>
        /// <param name="behaver"></param>
        /// <param name="currentAssignments"></param>
        /// <returns></returns>
        public override bool IsSatisfiedFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            AffinityComponent affinityComponent = BehaviorUtils.GetAffinityComponent(behaver);
            Optional<Score> maybeMovementSpeedScore = affinityComponent.AffinityFor(Commandment.Movement, Attribute.Speed);
            if (maybeMovementSpeedScore.HasValue)
            {
                // TODO: There's a way to make these comparable with operator overloading, p sure
                return maybeMovementSpeedScore.Value.CompareTo(Score.Min) > 0;
            } else
            {
                return false;
            }
        }

        public override Goal ResolveGoalFor(BehaviorComponent behaver)
        {
            // TODO: Get player component, return a MoveToPositionGoal with the player position
            throw new NotImplementedException();
        }
    }
}
*/