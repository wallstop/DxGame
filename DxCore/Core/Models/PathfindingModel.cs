using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Cache.Advanced;

namespace DxCore.Core.Models
{
    public sealed class PathfindingModel : Model
    {
        private ILoadingCache<MapDescriptor, NavigableMesh> NavigableMeshCache { get; }

        private NavigableMesh CurrentMesh { get; set; }

        private readonly ConcurrentQueue<PathfindingResponse> finishedResponses_;

        public PathfindingModel()
        {
            NavigableMeshCache =
                CacheBuilder<MapDescriptor, NavigableMesh>.NewBuilder()
                    .WithExpireAfterAccess(TimeSpan.FromMinutes(1))
                    .Build(descriptor => new NavigableMesh(descriptor));
            finishedResponses_ = new ConcurrentQueue<PathfindingResponse>();
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<PathFindingRequest>(HandlePathfindingRequest);
            base.OnAttach();
        }

        protected override void Update(DxGameTime gameTime)
        {
            MapModel mapModel = DxGame.Instance.Model<MapModel>();

            MapDescriptor currentDescriptor = mapModel?.Map?.MapDescriptor ?? null;
            if(!ReferenceEquals(currentDescriptor, null))
            {
                CurrentMesh = NavigableMeshCache.Get(currentDescriptor);
            }
            else
            {
                CurrentMesh = null;
            }

            PathfindingResponse completedPathfinding;
            while(finishedResponses_.TryDequeue(out completedPathfinding))
            {
                completedPathfinding.Emit();
            }
        }

        private void HandlePathfindingRequest(PathFindingRequest pathFindingRequest)
        {
            RequestPathfinding(CurrentMesh, pathFindingRequest, finishedResponses_);
        }

        private static void RequestPathfinding(NavigableMesh mesh, PathFindingRequest request,
            ConcurrentQueue<PathfindingResponse> finishedResponses)
        {
            Task<List<NavigableMeshNode>> futurePathfinding =
                new Task<List<NavigableMeshNode>>(() => mesh.PathFind(request.Start, request.Goal));

            futurePathfinding.Start();
            futurePathfinding.ContinueWith(pathfindingResult =>
            {
                PathfindingResponse response = new PathfindingResponse(pathfindingResult.Result, request.Requester);
                finishedResponses.Enqueue(response);
            });
        }
    }
}