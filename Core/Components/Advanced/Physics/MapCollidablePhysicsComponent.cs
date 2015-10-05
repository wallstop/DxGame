using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Utils;
using DXGame.Core.Map;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using NLog;

namespace DXGame.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public class MapCollidablePhysicsComponent : PhysicsComponent
    {
        private const int MAX_COLLISION_CHECKS = 10;
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly DxRectangleAreaComparer DXRECTANGLE_AREA_COMPARER = new DxRectangleAreaComparer();
        private DxVector2 Dimensions => space_.Dimensions;

        [DataMember]
        private readonly List<Tuple<MapCollidableComponent, TimeSpan>> mapTilesToIgnore_ = new List<Tuple<MapCollidableComponent, TimeSpan>>();
        
        private static readonly TimeSpan IGNORE_EXPIRY = TimeSpan.FromMilliseconds(30);

        private DxRectangle MapQueryRegion => Space;

        private MapCollidablePhysicsComponent(DxGame game, DxVector2 velocity, DxVector2 acceleration, SpatialComponent space, UpdatePriority updatePriority)
            : base(game, velocity, acceleration, space, updatePriority)
        {
            MessageHandler.RegisterMessageHandler<DropThroughPlatformRequest>(HandleDropThroughPlatformRequest);
        }

        public new static MapCollidablePhysicsComponentBuilder Builder()
        {
            return new MapCollidablePhysicsComponentBuilder();
        }

        public class MapCollidablePhysicsComponentBuilder : PhysicsComponentBuilder
        {
            public override PhysicsComponent Build()
            {
                CheckParameters();
                var physics = new MapCollidablePhysicsComponent(game_, velocity_, acceleration_, space_, updatePriority_);
                foreach (var force in forces_)
                {
                    physics.AttachForce(force);
                }
                return physics;
            }
        }

        private void HandleDropThroughPlatformRequest(DropThroughPlatformRequest message)
        {
            var map = DxGame.Model<MapModel>();
            var mapQueryRegion = MapQueryRegion;
            /* Give the region a little bit of buffer room to check for platforms we may be standing on */
            mapQueryRegion.Height += 3;
            List<MapCollidableComponent> mapTiles = map.Map.Collidables.InRange(mapQueryRegion);
            /* TODO: Either find or implement an LRU cache */
            mapTilesToIgnore_.AddRange(mapTiles.Where(tile => tile.PlatformType == PlatformType.Platform).Select(tile => Tuple.Create(tile, DxGame.CurrentTime.TotalGameTime)));
        }

        protected override void Update(DxGameTime gameTime)
        {
            /*
                Perform the normal PhysicsComponent's update first. We assume that at the end of our last update cycle,
                we are collision free. The PhysicsComponent's update cycle may place us into some kind of state where
                we're colliding with the map, so we need to handle that.
            */
            base.Update(gameTime);

            var map = DxGame.Model<MapModel>();
            var mapQueryRegion = MapQueryRegion;

            List<MapCollidableComponent> mapTiles = map.Map.Collidables.InRange(mapQueryRegion);
            mapTilesToIgnore_.RemoveAll(mapTile => !mapTiles.Contains(mapTile.Item1) && (mapTile.Item2 + IGNORE_EXPIRY) < gameTime.TotalGameTime);

            CollisionMessage collision = new CollisionMessage();

            // TODO: Properly handle buggy collisions (failsafe for now)
            for(int i = 0; i < MAX_COLLISION_CHECKS; ++i)
            {
                var largestIntersectionTuple = FindLargestIntersection(mapTiles, Space);
                if (largestIntersectionTuple == null)
                {
                    break;
                }
                var largestIntersection = largestIntersectionTuple.Item1;
                var mapSpatial = largestIntersectionTuple.Item2;
                var mapBlockPosition = mapSpatial.Spatial.Position;
                var mapBlockDimensions = mapSpatial.Spatial.Dimensions;

                if (mapSpatial.CollidesWith(Velocity) && !mapTilesToIgnore_.Any(spatial => Equals(spatial.Item1, mapSpatial)))
                {
                    /*
                        Wrap to the Y axis if the collision area was greater along the X axis (implies the collision occured either up or down,
                        as a left/right collision *should* have very little overlap on the X axis.

                        OR wrap to the Y axis if we weren't moving in the X direction. This prevents "warps", where the player is jumping
                        something like a few pixels off of a block boundary and gets warped back.

                        Since the acceleration from gravity is always going to throw us downward, we check this first.
                    */
                    if (largestIntersection.Width >= largestIntersection.Height ||
                        MathUtils.FuzzyCompare(Velocity.X, 0.0f) == 0)
                    {
                        // below collision
                        if (Position.Y + Dimensions.Y >= mapBlockPosition.Y && mapBlockPosition.Y > Position.Y)
                        {
                            Position = new DxVector2(Position.X, mapBlockPosition.Y - Dimensions.Y);
                            collision.CollisionDirections.Add(Direction.South);
                        }
                        else // above collision
                        {
                            Position = new DxVector2(Position.X, mapBlockPosition.Y + mapBlockDimensions.Y);
                            collision.CollisionDirections.Add(Direction.North);
                        }
                    }
                    /*
                            Wrap to the X axis otherwise. 
                    */
                    else
                    // if (intersection.Height > intersection.Width || MathUtils.FuzzyCompare(Velocity.Y, 0.0, MathUtils.DoubleTolerance) == 0)
                    {
                        // left collision 
                        if (Position.X < mapBlockPosition.X + mapBlockDimensions.X && mapBlockPosition.X < Position.X)
                        {
                            Position = new DxVector2(Position.X + largestIntersection.Width, Position.Y);
                            collision.CollisionDirections.Add(Direction.West);
                        }
                        // right collision
                        else
                        {
                            Position = new DxVector2(Position.X - largestIntersection.Width, Position.Y);
                            collision.CollisionDirections.Add(Direction.East);
                        }
                    }
                }
                else
                {
                    mapTiles.Remove(mapSpatial);
                }
            }

            // Let everyone else know we collided (only if we collided with anything)
            if (collision.CollisionDirections.Any())
            {
                Parent.BroadcastMessage(collision);
            }
        }

        private static Tuple<DxRectangle, MapCollidableComponent> FindLargestIntersection(
            IEnumerable<MapCollidableComponent> mapTiles,
            DxRectangle currentSpace)
        {
            Tuple<DxRectangle, MapCollidableComponent> largestIntersection = null;
            foreach (var mapTile in mapTiles)
            {
                if (!currentSpace.Intersects(mapTile.Spatial.Space))
                {
                    continue;
                }
                // There's a bug here that causes like infinite collision detection, fix pls
                var intersection = DxRectangle.Intersect(mapTile.Spatial.Space, currentSpace);
                if (largestIntersection == null || DXRECTANGLE_AREA_COMPARER.Compare(intersection,
                    largestIntersection.Item1) <= 0)
                {
                    largestIntersection = Tuple.Create(intersection, mapTile);
                }
            }
            return largestIntersection;
        }
    }
}