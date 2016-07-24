using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils.Cache.Advanced;

namespace DxCore.Core.Services
{
    public sealed class PathfindingService : Service
    {
        private ILoadingCache<MapDescriptor, NavigableMesh> NavigableMeshCache { get; }

        private NavigableMesh CurrentMesh { get; set; }

        private readonly ConcurrentQueue<PathfindingResponse> finishedResponses_;

        public PathfindingService()
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
            MapService mapService = DxGame.Instance.Service<MapService>();

            MapDescriptor currentDescriptor = mapService?.Map?.MapDescriptor ?? null;
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