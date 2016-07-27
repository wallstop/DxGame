using System.Collections.Concurrent;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Services.Components
{
    public sealed class PathfindingResponseDispatcher : Component
    {
        private ConcurrentQueue<PathfindingResponse> FinishedResponses { get; }

        public PathfindingResponseDispatcher(ConcurrentQueue<PathfindingResponse> finishedResponses)
        {
            Validate.Hard.IsNotNull(finishedResponses);
            FinishedResponses = finishedResponses;
        }

        protected override void Update(DxGameTime gameTime)
        {
            PathfindingResponse completedPathfinding;
            while(FinishedResponses.TryDequeue(out completedPathfinding))
            {
                completedPathfinding.Emit();
            }
        }
    }
}
