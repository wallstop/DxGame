using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
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
        private static readonly TimeSpan IGNORE_EXPIRY = TimeSpan.FromMilliseconds(30);

        [DataMember] private readonly List<Tuple<MapCollidableComponent, TimeSpan>> mapTilesToIgnore_ =
            new List<Tuple<MapCollidableComponent, TimeSpan>>();

        private DxVector2 Dimensions => space_.Dimensions;

        protected MapCollidablePhysicsComponent(DxGame game, DxVector2 velocity, DxVector2 acceleration,
            SpatialComponent space, UpdatePriority updatePriority)
            : base(game, velocity, acceleration, space, updatePriority)
        {
            MessageHandler.RegisterMessageHandler<DropThroughPlatformRequest>(HandleDropThroughPlatformRequest);
        }

        public new static MapCollidablePhysicsComponentBuilder Builder()
        {
            return new MapCollidablePhysicsComponentBuilder();
        }

        private void HandleDropThroughPlatformRequest(DropThroughPlatformRequest message)
        {
            var map = DxGame.Model<MapModel>();
            var mapQueryRegion = Space;
            /* Give the region a little bit of buffer room to check for platforms we may be standing on */
            mapQueryRegion.Height += 3;
            List<MapCollidableComponent> mapTiles = map.Map.Collidables.InRange(mapQueryRegion);
            /* TODO: Either find or implement an LRU cache */
            mapTilesToIgnore_.AddRange(
                mapTiles.Where(tile => tile.PlatformType == PlatformType.Platform)
                    .Select(tile => Tuple.Create(tile, DxGame.CurrentTime.TotalGameTime)));
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

        protected override void Update(DxGameTime gameTime)
        {
            var previousPosition = Position;
            /*
                Perform the normal PhysicsComponent's update first. We assume that at the end of our last update cycle,
                we are collision free. The PhysicsComponent's update cycle may place us into some kind of state where
                we're colliding with the map, so we need to handle that.
            */
            base.Update(gameTime);
            var traveledLine = new DxLine(previousPosition, Position);

            // TODO: Vector-based collision

            var map = DxGame.Model<MapModel>();
            var collisionSpace = CollisionSpace(traveledLine);

            List<MapCollidableComponent> mapTiles = map.Map.Collidables.InRange(collisionSpace);
            mapTilesToIgnore_.RemoveAll(
                mapTile => !mapTiles.Contains(mapTile.Item1) && (mapTile.Item2 + IGNORE_EXPIRY) < gameTime.TotalGameTime);

            var collision = new CollisionMessage();

            // TODO: Properly handle buggy collisions (failsafe for now)
            for (int i = 0; i < MAX_COLLISION_CHECKS; ++i)
            {
                var largestIntersectionTuple = FindLargestIntersection(mapTiles, collisionSpace);
                if (largestIntersectionTuple == null)
                {
                    break;
                }
                var largestIntersection = largestIntersectionTuple.Item1;
                var mapSpatial = largestIntersectionTuple.Item2;
                var mapBlockPosition = mapSpatial.Spatial.Position;
                var mapBlockDimensions = mapSpatial.Spatial.Dimensions;

                var direction = traveledLine.Vector;
                if (mapSpatial.CollidesWith(direction) &&
                    !mapTilesToIgnore_.Any(spatial => Equals(spatial.Item1, mapSpatial)))
                {
                    /*
                        Wrap to the Y axis if the collision area was greater along the X axis (implies the collision occured either up or down,
                        as a left/right collision *should* have very little overlap on the X axis.

                        OR wrap to the Y axis if we weren't moving in the X direction. This prevents "warps", where the player is jumping
                        something like a few pixels off of a block boundary and gets warped back.

                        Since the acceleration from gravity is always going to throw us downward, we check this first.
                    */
                    if (largestIntersection.Width >= largestIntersection.Height ||
                        Velocity.X.FuzzyCompare(0.0f) == 0)
                    {
                        // below collision
                        if (collisionSpace.Y + collisionSpace.Height >= mapBlockPosition.Y && mapBlockPosition.Y > collisionSpace.Height && previousPosition.Y + Dimensions.Y <= mapBlockPosition.Y && mapSpatial.CollidableDirections.Contains(CollidableDirection.Up))
                        {
                            Position = new DxVector2(Position.X, mapBlockPosition.Y - Dimensions.Y);
                            collision.CollisionDirections.Add(Direction.South);
                        }
                        // above collision
                        else if (mapSpatial.CollidableDirections.Contains(CollidableDirection.Down)) 
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
                        if (collisionSpace.X < mapBlockPosition.X + mapBlockDimensions.X && mapBlockPosition.X < collisionSpace.X && mapSpatial.CollidableDirections.Contains(CollidableDirection.Right))
                        {
                            Position = new DxVector2(Position.X + largestIntersection.Width, Position.Y);
                            collision.CollisionDirections.Add(Direction.West);
                        }
                        // right collision
                        else if(mapSpatial.CollidableDirections.Contains(CollidableDirection.Left))
                        {
                            Position = new DxVector2(Position.X - largestIntersection.Width, Position.Y);
                            collision.CollisionDirections.Add(Direction.East);
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
            if (collision.CollisionDirections.Any())
            {
                Parent?.BroadcastMessage(collision);
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
    }
}