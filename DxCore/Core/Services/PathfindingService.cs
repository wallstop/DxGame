using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Services.Components;
using NLog;
using WallNetCore.Cache.Advanced;
using WallNetCore.Validate;

namespace DxCore.Core.Services
{
    public sealed class PathfindingService : DxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private NavigableMesh CurrentMesh { get; set; }

        private ConcurrentQueue<PathfindingResponse> FinishedResponses { get; }

        private ILoadingCache<MapDescriptor, NavigableMesh> NavigableMeshCache { get; }

        private PathfindingResponseDispatcher ResponseDispatcher { get; set; }

        public PathfindingService()
        {
            NavigableMeshCache =
                CacheBuilder<MapDescriptor, NavigableMesh>.NewBuilder()
                    .WithExpireAfterAccess(TimeSpan.FromMinutes(1))
                    .Build(descriptor => new NavigableMesh(descriptor));
            FinishedResponses = new ConcurrentQueue<PathfindingResponse>();
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(ResponseDispatcher))
            {
                ResponseDispatcher = new PathfindingResponseDispatcher(FinishedResponses);
                Self.AttachComponent(ResponseDispatcher);
            }

            Self.MessageHandler.RegisterMessageHandler<PathfindingRequest>(HandlePathfindingRequest);
            Self.MessageHandler.RegisterMessageHandler<MapRotationNotification>(HandleMapRotationNotification);
        }

        private void HandleMapRotationNotification(MapRotationNotification mapRotation)
        {
            Map.Map map = mapRotation.Map;
            if(!ReferenceEquals(map?.MapDescriptor, null))
            {
                CurrentMesh = NavigableMeshCache.Get(map.MapDescriptor);
            }
            else
            {
                CurrentMesh = null;
            }
        }

        private void HandlePathfindingRequest(PathfindingRequest pathFindingRequest)
        {
            if(Validate.Check.IsNull(CurrentMesh))
            {
                Logger.Debug("Ignoring {0}; null mesh :(", pathFindingRequest);
                return;
            }
            RequestPathfinding(CurrentMesh, pathFindingRequest, FinishedResponses);
        }

        private static void RequestPathfinding(NavigableMesh mesh, PathfindingRequest request,
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