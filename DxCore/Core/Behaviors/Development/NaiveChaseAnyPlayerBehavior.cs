using System.Collections.Generic;
using DXGame.Core.Behaviors.Goals;
using DxCore.Core.Utils;
using DxCore.Core.Messaging;
using DxCore;
using System.Linq;
using DxCore.Core.Services;
using DxCore.Core;
using DxCore.Core.Components.Advanced.Behaviors;

namespace DXGame.Core.Behaviors.Development
{
    class NaiveChaseAnyPlayerBehavior : Behavior
    {
        public Score FitnessFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            // Fitness scales directly with movement speed
            AffinityComponent affinityComponent = behaver.AffinityComponent();
            //? 'Movement' vs. Move + cardinal directions? 
            Optional<Score> maybeMovementSpeedScore = affinityComponent.AffinityFor(Commandment.Movement, Attribute.Speed);
            return maybeMovementSpeedScore.HasValue ? maybeMovementSpeedScore.Value : Score.Min;
        }

        /// <summary>
        /// The chasing behavior applies to all entities who can move, expressed as any non-zero score for Movement:Speed
        /// </summary>
        /// <param name="behaver"></param>
        /// <param name="currentAssignments"></param>
        /// <returns></returns>
        public bool SatisfiedFor(BehaviorComponent behaver, Dictionary<BehaviorComponent, Behavior> currentAssignments)
        {
            AffinityComponent affinityComponent = behaver.AffinityComponent();
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

        public Goal ResolveGoalFor(BehaviorComponent behaver)
        {
            // TODO: Obsolete; how does 'PlayerService' work? 
            PlayerService playerService = DxGame.Instance.Service<PlayerService>();
            // Chase *any* player, hence the name
            Player player = playerService.Players.First();

            return new MoveToPositionGoal(player.Position.Center);
        }

        public override string ToString()
        {
            return "NaiveChaseAnyPlayerBehavior";
        }
    }
}
