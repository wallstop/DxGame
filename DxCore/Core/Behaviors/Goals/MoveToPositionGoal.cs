using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DXGame.Core.Components.Advanced.Behaviors;
using DxCore.Core.Components.Advanced.Physics;

namespace DXGame.Core.Behaviors.Goals
{
    class MoveToPositionGoal : Goal
    {
        private DxVector2 TargetPosition { get; }
        // Use a flag to ensure we only emit a single pathfinding message
        private bool pathfindingStarted_ = false;

        public MoveToPositionGoal(DxVector2 targetPosition)
        {
            TargetPosition = targetPosition;
        }

        public override void Update(DxGameTime gameTime, BehaviorComponent behaver)
        {
            if (!pathfindingStarted_)
            {
                DxVector2 currentPosition = behaver.Parent.ComponentOfType<PhysicsComponent>().Position;
                PathfindingRequest pathfindingRequest = new PathfindingRequest(currentPosition, TargetPosition, behaver.Parent.Id);
                pathfindingRequest.Emit();

                pathfindingStarted_ = true;
            }
        }
    }
}
