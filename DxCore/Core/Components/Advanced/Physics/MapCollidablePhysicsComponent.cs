using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Utils;
using DxCore.Core.Map;
using DxCore.Core.Messaging;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced.Physics
{
    [DataContract]
    [Serializable]
    public class MapCollidablePhysicsComponent : PhysicsComponent
    {
        private const int MAX_COLLISION_CHECKS = 20;
        private static readonly DxRectangleAreaComparer DXRECTANGLE_AREA_COMPARER = new DxRectangleAreaComparer();
        private static readonly TimeSpan IGNORE_EXPIRY = TimeSpan.FromMilliseconds(30);

        private DxVector2 Dimensions => space_.Dimensions;

        [DataMember]
        private Dictionary<TileType, TimeSpan> ignoreCollision_ = new Dictionary<TileType, TimeSpan>(); 

        protected MapCollidablePhysicsComponent(DxVector2 velocity, DxVector2 acceleration, SpatialComponent space,
            UpdatePriority updatePriority) : base(velocity, acceleration, space, updatePriority)
        {
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<DropThroughPlatformRequest>(HandleDropThroughPlatformRequest);
            base.OnAttach();
        }

        public new static MapCollidablePhysicsComponentBuilder Builder()
        {
            return new MapCollidablePhysicsComponentBuilder();
        }

        private void HandleDropThroughPlatformRequest(DropThroughPlatformRequest message)
        {
            ignoreCollision_[TileType.Platform] = IGNORE_EXPIRY;
        }

        /**
            Given a line segment that represents "the arc that was traveled" (should be a straight line),
            determines the "full bounding box" that we occupied while traveling. While this is kind of hacky,
            this allows us the freedom of "fuzzier" collision detection. IE, the case when we have a REALLY
            long frame that we end up traveling VERY far in, like, through the map.
        */

        private DxRectangle CollisionSpace(DxLine travelArc)
        {
            var startX = Math.Min(travelArc.Start.X, travelArc.End.X);
            var stopX = Math.Max(travelArc.Start.X, travelArc.End.X) + Space.Width;
            var startY = Math.Min(travelArc.Start.Y, travelArc.End.Y);
            var stopY = Math.Max(travelArc.Start.Y, travelArc.End.Y) + Space.Height;
            return new DxRectangle(startX, startY, stopX - startX, stopY - startY);
        }

        private void UpdateCollisionTimeouts(DxGameTime gameTime)
        {
            foreach(TileType tileType in ignoreCollision_.Keys.ToArray())
            {
                TimeSpan existingTimeout = ignoreCollision_[tileType];
                existingTimeout -= gameTime.ElapsedGameTime;
                if(existingTimeout < TimeSpan.Zero)
                {
                    ignoreCollision_.Remove(tileType);
                }
                else
                {
                    ignoreCollision_[tileType] = existingTimeout;
                }
            }
        }

        protected override void Update(DxGameTime gameTime)
        {
            UpdateCollisionTimeouts(gameTime);

            var previousPosition = Position;
            /*
                Perform the normal PhysicsComponent's update first. We assume that at the end of our last update cycle,
                we are collision free. The PhysicsComponent's update cycle may place us into some kind of state where
                we're colliding with the map, so we need to handle that.
            */
            base.Update(gameTime);
            var traveledLine = new DxLine(previousPosition, Position);

            // TODO: Vector-based collision

            var map = DxGame.Instance.Model<MapModel>();
            var collisionSpace = CollisionSpace(traveledLine);

            List<MapCollidable> mapTiles = map.Map.Collidables.InRange(collisionSpace);

            if(ignoreCollision_.Any())
            {
                mapTiles.RemoveAll(mapCollidable => ignoreCollision_.ContainsKey(mapCollidable.Tile.Type));
            }

            CollisionMessage collision = new CollisionMessage();

            // TODO: Properly handle buggy collisions (failsafe for now)
            for(int i = 0; i < MAX_COLLISION_CHECKS; ++i)
            {
                var largestIntersectionTuple = FindLargestIntersection(mapTiles, collisionSpace, previousPosition);
                if(largestIntersectionTuple == null)
                {
                    break;
                }
                var largestIntersection = largestIntersectionTuple.Item1;
                var mapSpatial = largestIntersectionTuple.Item2;
                var mapBlockPosition = mapSpatial.Space.Position;
                var mapBlockDimensions = mapSpatial.Space.Dimensions;
                const float dimensionScalar = 0.9f;
                var direction = traveledLine.Vector;
                if(mapSpatial.Tile.CollidesWith(direction))
                {
                    if(largestIntersection.Width >= largestIntersection.Height || Velocity.X.FuzzyCompare(0, 0.01f) == 0)
                    {
                        if(previousPosition.Y + dimensionScalar * Dimensions.Y <= mapBlockPosition.Y &&
                           mapSpatial.Tile.CollidableDirections.Contains(CollidableDirection.Up))
                        {
                            Position = new DxVector2(Position.X, mapBlockPosition.Y - Dimensions.Y);
                            collision.CollisionDirections[Direction.South] = mapSpatial;
                        }
                        else if(previousPosition.Y >= mapBlockPosition.Y + dimensionScalar * mapBlockDimensions.Y &&
                                mapSpatial.Tile.CollidableDirections.Contains(CollidableDirection.Down))
                        {
                            Position = new DxVector2(Position.X, mapBlockPosition.Y + mapBlockDimensions.Y);
                            collision.CollisionDirections[Direction.North] = mapSpatial;
                        }
                        else
                        {
                            mapTiles.Remove(mapSpatial);
                        }
                    }
                    else
                    {
                        if(previousPosition.X >= mapBlockPosition.X + dimensionScalar * mapBlockDimensions.X &&
                           mapSpatial.Tile.CollidableDirections.Contains(CollidableDirection.Right))
                        {
                            Position = new DxVector2(mapBlockPosition.X + mapBlockDimensions.X, Position.Y);
                            collision.CollisionDirections[Direction.West] = mapSpatial;
                        }
                        else if(previousPosition.X + dimensionScalar * Dimensions.X <= mapBlockPosition.X &&
                                mapSpatial.Tile.CollidableDirections.Contains(CollidableDirection.Left))
                        {
                            Position = new DxVector2(mapBlockPosition.X - Dimensions.X, Position.Y);
                            collision.CollisionDirections[Direction.West] = mapSpatial;
                        }
                        else
                        {
                            mapTiles.Remove(mapSpatial);
                        }
                    }

                    /* Since we just collided, recalculate our "collision space" */
                    traveledLine = new DxLine(previousPosition, Position);
                    collisionSpace = CollisionSpace(traveledLine);
                }
                else
                {
                    mapTiles.Remove(mapSpatial);
                }
            }

            // Let everyone else know we collided (only if we collided with anything)
            if(collision.CollisionDirections.Any())
            {
                collision.Target = Parent?.Id;
                collision.Emit();
            }
        }

        private static Tuple<DxRectangle, MapCollidable> FindLargestIntersection(
            IEnumerable<MapCollidable> mapTiles, DxRectangle currentSpace, DxVector2 originalSpace)
        {
            Tuple<DxRectangle, MapCollidable> largestIntersection = null;
            foreach(var mapTile in mapTiles)
            {
                if(!currentSpace.Intersects(mapTile.Space))
                {
                    continue;
                }
                // There's a bug here that causes like infinite collision detection, fix pls
                DxRectangle intersection = DxRectangle.Intersect(mapTile.Space, currentSpace);
                if(largestIntersection == null ||
                   DXRECTANGLE_AREA_COMPARER.Compare(intersection, largestIntersection.Item1) <= 0)
                {
                    largestIntersection = Tuple.Create(intersection, mapTile);
                }
            }
            return largestIntersection;
        }

        public class MapCollidablePhysicsComponentBuilder : PhysicsComponentBuilder
        {
            public override PhysicsComponent Build()
            {
                var physics = new MapCollidablePhysicsComponent(velocity_, acceleration_, space_, updatePriority_);
                foreach(var force in forces_)
                {
                    physics.AttachForce(force);
                }
                return physics;
            }
        }
    }
}