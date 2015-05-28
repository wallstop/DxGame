using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Utils;
using DXGame.Core.Messaging;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Advanced.Physics
{
    [Serializable]
    [DataContract]
    public class MapCollideablePhysicsComponent : PhysicsComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapCollideablePhysicsComponent));
        private static readonly DxRectangleAreaComparer DXRECTANGLE_AREA_COMPARER = new DxRectangleAreaComparer();

        private DxRectangle Space
        {
            get { return ((SpatialComponent) position_).Space; }
        }

        private DxVector2 Dimensions
        {
            get { return ((SpatialComponent) position_).Dimensions; }
        }

        private DxRectangle MapQueryRegion
        {
            get
            {
                int blockSize = DxGame.Model<MapModel>().BlockSize;
                return new DxRectangle(Space.X - blockSize, Space.Y - blockSize, Space.Width + blockSize * 2,
                    Space.Height + blockSize * 2);
            }
        }

        public MapCollideablePhysicsComponent(DxGame game)
            : base(game)
        {
        }

        public MapCollideablePhysicsComponent WithSpatialComponent(SpatialComponent space)
        {
            Validate.IsNotNull(space, $"Cannot initialize {GetType()} with a null SpatialComponent");
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
            IEnumerable<SpatialComponent> mapTiles = map.SpatialsInRange(MapQueryRegion);

            CollisionMessage collision = new CollisionMessage();
            SortedDictionary<DxRectangle, SpatialComponent> intersections;

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
                    if (Position.Y + Dimensions.Y > mapBlockPosition.Y && mapBlockPosition.Y > Position.Y)
                    {
                        Position = new DxVector2(Position.X, mapBlockPosition.Y - Dimensions.Y);
                        collision.CollisionDirections.Add(CollisionDirection.South);
                    }
                    else // above collision
                    {
                        Position = new DxVector2(Position.X, mapBlockPosition.Y + mapBlockDimensions.Y);
                        collision.CollisionDirections.Add(CollisionDirection.North);
                    }
                    //Velocity = new DxVector2(Velocity.X, 0);
                    //Acceleration = new DxVector2(Acceleration.X, 0);
                }
                /*
                        Wrap to the X axis otherwise. 
                */
                else
                // if (intersection.Height > intersection.Width || MathUtils.FuzzyCompare(Velocity.Y, 0.0, MathUtils.DoubleTolerance) == 0)
                {
                    // left collision // TODO: Fix this. This left-first case causes a bug of "never being able to move left" while jumping
                    if (Position.X < mapBlockPosition.X + mapBlockDimensions.X && mapBlockPosition.X < Position.X)
                    {
                        Position = new DxVector2(mapBlockPosition.X + mapBlockDimensions.X, Position.Y);
                        collision.CollisionDirections.Add(CollisionDirection.West);
                    }
                    else // right collision
                    {
                        Position = new DxVector2(mapBlockPosition.X - Dimensions.X, Position.Y);
                        collision.CollisionDirections.Add(CollisionDirection.East);
                    }
                    //Velocity = new DxVector2(0, Velocity.Y);
                    //Acceleration = new DxVector2(0, Acceleration.Y);
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
            with smallest intersections appearing first.
        */

        private static SortedDictionary<DxRectangle, SpatialComponent> FindIntersections(
            IEnumerable<SpatialComponent> mapTiles, DxRectangle currentSpace)
        {
            SortedDictionary<DxRectangle, SpatialComponent> intersections =
                new SortedDictionary<DxRectangle, SpatialComponent>(DXRECTANGLE_AREA_COMPARER);
            foreach (SpatialComponent spatial in mapTiles)
            {
                if (!currentSpace.Intersects(spatial.Space))
                {
                    continue;
                }
                var intersection = DxRectangle.Intersect(spatial.Space, currentSpace);
                if (!intersections.ContainsKey(intersection))
                {
                    intersections.Add(intersection, spatial);
                }
            }
            return intersections;
        }
    }
}