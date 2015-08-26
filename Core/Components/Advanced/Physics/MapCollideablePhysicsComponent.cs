using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Map;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Utils;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using NLog;

namespace DXGame.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public class MapCollideablePhysicsComponent : PhysicsComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        private static readonly int QUERY_BUFFER = 50; /* Pixels around the Spatial to check for collisions */
        private static readonly DxRectangleAreaComparer DXRECTANGLE_AREA_COMPARER = new DxRectangleAreaComparer();
        private DxRectangle Space => ((SpatialComponent) position_).Space;
        private DxVector2 Dimensions => ((SpatialComponent) position_).Dimensions;

        private DxRectangle MapQueryRegion
            => new DxRectangle(Space.X - QUERY_BUFFER, Space.Y - QUERY_BUFFER, Space.Width + QUERY_BUFFER * 2,
                Space.Height + QUERY_BUFFER * 2);

        public MapCollideablePhysicsComponent(DxGame game)
            : base(game)
        {
        }

        public MapCollideablePhysicsComponent WithSpatialComponent(SpatialComponent space)
        {
            Validate.IsNotNull(space, StringUtils.GetFormattedNullOrDefaultMessage(this, space));
            position_ = space;
            return this;
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

            List<CollidableComponent> mapTiles = map.Map.Collidables.InRange(mapQueryRegion);

            CollisionMessage collision = new CollisionMessage();
            SortedDictionary<DxRectangle, CollidableComponent> intersections;
            do
            {
                intersections = FindIntersections(mapTiles, Space);
                if (!intersections.Any())
                {
                    continue;
                }
                var largestIntersection = intersections.Keys.Last();
                var mapSpatial = intersections.Values.Last();
                var mapBlockPosition = mapSpatial.Position;
                var mapBlockDimensions = mapSpatial.Dimensions;
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
                        collision.CollisionDirections.Add(CollisionDirection.South);
                    }
                    else // above collision
                    {
                        Position = new DxVector2(Position.X, mapBlockPosition.Y + mapBlockDimensions.Y);
                        collision.CollisionDirections.Add(CollisionDirection.North);
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
                        if (mapSpatial.CollidableDirections.Contains(CollidableDirection.Right))
                        {
                            Position = new DxVector2(Position.X + largestIntersection.Width, Position.Y);
                            collision.CollisionDirections.Add(CollisionDirection.West);
                        }
                    }
                    // right collision
                    else if (mapSpatial.CollidableDirections.Contains(CollidableDirection.Left))
                    {
                        Position = new DxVector2(Position.X - largestIntersection.Width, Position.Y);
                        collision.CollisionDirections.Add(CollisionDirection.East);
                    }
                }
            } while (intersections.Any());

            // Let everyone else know we collided (only if we collided with anything)
            if (collision.CollisionDirections.Any())
            {
                Parent.BroadcastMessage(collision);
            }
        }

        /*
            Given a list of SpatialComponents (maptiles) and a space, returns a sorted dictionary of intersections, 
            with smallest intersections appearing first. This lets us prioritize
        */

        // TODO: Change this to ditch the sorteddictionary. We actually only care about the largest intersection, so just find that instead.
        private static SortedDictionary<DxRectangle, CollidableComponent> FindIntersections(
            IEnumerable<CollidableComponent> mapTiles, DxRectangle currentSpace)
        {
            SortedDictionary<DxRectangle, CollidableComponent> intersections =
                new SortedDictionary<DxRectangle, CollidableComponent>(DXRECTANGLE_AREA_COMPARER);
            foreach (var spatial in mapTiles)
            {
                if (!currentSpace.Intersects(spatial.Space))
                {
                    continue;
                }
                var intersection = DxRectangle.Intersect(spatial.Space, currentSpace);
                // If we have duplicates, that's ok, they'll be picked up next time
                if (!intersections.ContainsKey(intersection))
                {
                    intersections.Add(intersection, spatial);
                }
            }
            return intersections;
        }

        private static RectangleSpatialPair? FindLargestIntersection(IEnumerable<SpatialComponent> mapTiles,
            DxRectangle currentSpace)
        {
            RectangleSpatialPair largestIntersection = new RectangleSpatialPair();
            foreach (SpatialComponent mapTile in mapTiles)
            {
                if (!currentSpace.Intersects(mapTile.Space))
                {
                    continue;
                }
                var intersection = DxRectangle.Intersect(mapTile.Space, currentSpace);
                if (DXRECTANGLE_AREA_COMPARER.Compare(intersection, largestIntersection.Rectangle) > 0)
                {
                    largestIntersection.Rectangle = intersection;
                    largestIntersection.Spatial = mapTile;
                }
            }
            return largestIntersection;
        }

        private struct RectangleSpatialPair
        {
            public DxRectangle Rectangle;
            public SpatialComponent Spatial;
        }
    }
}