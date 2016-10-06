using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DXGame.Core.Components.Advanced.Behaviors;
using DxCore.Core.Components.Advanced.Physics;
using System;
using DxCore.Core.Utils;
using NLog;

namespace DXGame.Core.Behaviors.Goals
{
    class MoveToPositionGoal : Goal
    {
        private static readonly double ARRIVAL_THRESHOLD = 50;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private DxVector2 TargetPosition { get; }
        private TimeSpan TimeLimit { get; }

        private TimeSpan pathfindingStartTime_;
        // Use a flag to ensure we only emit a single pathfinding message
        private bool pathfindingStarted_ = false;

        public MoveToPositionGoal(DxVector2 targetPosition) : this(targetPosition, TimeSpan.FromSeconds(3)) { }

        public MoveToPositionGoal(DxVector2 targetPosition, TimeSpan timeLimit)
        {
            TargetPosition = targetPosition;
            TimeLimit = timeLimit;
        }

        public override void Update(DxGameTime gameTime, BehaviorComponent behaver)
        {
            // If we haven't started pathfinding to our target yet, begin immediately
            if (!pathfindingStarted_)
            {
                DxVector2 currentPosition = behaver.Parent.ComponentOfType<PhysicsComponent>().Position;
                PathfindingRequest pathfindingRequest = new PathfindingRequest(currentPosition, TargetPosition, behaver.Parent.Id);
                pathfindingRequest.Emit();

                pathfindingStarted_ = true;
                pathfindingStartTime_ = gameTime.TotalGameTime;
            } // If we're already on our way, check if we've arrived or if we're out of time
            else if (HaveArrived(behaver) || OutOfTime(gameTime))
            {
                MarkSuccessful();
            }
            // TODO: Do pathfinding messages dedupe or whatever?  Do we need to do more complex teardown here? 
        }

        private bool HaveArrived(BehaviorComponent behaver)
        {
            // TODO: Can we query the PathfindingService instead of working out our own "arrival" heuristic? 
            DxVector2 currentPosition = behaver.Parent.ComponentOfType<PhysicsComponent>().Position;
            float xDelta = MathUtils.Delta(currentPosition.X, TargetPosition.X);
            float yDelta = MathUtils.Delta(currentPosition.Y, TargetPosition.Y);

            return xDelta < ARRIVAL_THRESHOLD && yDelta < ARRIVAL_THRESHOLD;
        }

        private bool OutOfTime(DxGameTime gameTime)
        {
            TimeSpan timeSpent = gameTime.TotalGameTime - pathfindingStartTime_;
            return TimeLimit < timeSpent;
        }
    }
}
