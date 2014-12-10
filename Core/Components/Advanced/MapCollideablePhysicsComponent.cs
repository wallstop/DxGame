using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Models;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class MapCollideablePhysicsComponent : PhysicsComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MapCollideablePhysicsComponent));

        public MapCollideablePhysicsComponent(GameObject parent = null)
            : base(parent)
        {
        }

        private Vector2 Position
        {
            get { return position_.Position; }
            set { position_.Position = value; }
        }

        private Rectangle Space
        {
            get { return ((SpatialComponent) position_).Space; }
        }

        private Vector2 Dimensions
        {
            get { return ((SpatialComponent) position_).Dimensions; }
        }

        private Rectangle MapQueryRegion
        {
            get
            {
                int blockSize = GameState.Model<MapModel>().BlockSize;
                return new Rectangle(Space.X - blockSize, Space.Y - blockSize, Space.Width + blockSize * 2,
                    Space.Height + blockSize * 2);
            }
        }

        public MapCollideablePhysicsComponent WithSpatialComponent(SpatialComponent space)
        {
            Debug.Assert(space != null,
                "MapCollideablePhysicsComponent cannot be initialized with a null SpatialComponent.");
            position_ = space;
            return this;
        }

        public override bool Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Rectangle oldSpace = Space;
            Vector2 oldPosition = Position;
            Vector2 oldDimensions = Dimensions;

            var map = GameState.Model<MapModel>();
            IEnumerable<SpatialComponent> mapTiles =
                map.SpatialsInRange(MapQueryRegion);

            Vector2 newPosition = Position;
            Vector2 newVelocity = Velocity;
            Vector2 newAcceleration = Acceleration;
            foreach (SpatialComponent spatial in mapTiles)
            {
                if (oldSpace.Intersects(spatial.Space))
                {
                    Vector2 mapBlockPosition = spatial.Position;
                    Vector2 mapBlockDimensions = spatial.Dimensions;
                    Rectangle intersection = Rectangle.Intersect(spatial.Space, oldSpace);
                    if (intersection.Width > intersection.Height)
                    {
                        // below collision
                        if (oldPosition.Y + oldDimensions.Y > mapBlockPosition.Y && mapBlockPosition.Y > oldPosition.Y)
                        {
                            newPosition.Y = mapBlockPosition.Y - oldDimensions.Y;
                        }
                        else // above collision
                        {
                            newPosition.Y = mapBlockPosition.Y + mapBlockDimensions.Y;
                        }
                        newVelocity.Y = 0;
                        newAcceleration.Y = 0;
                    } 
                    else if (intersection.Height > intersection.Width)
                    {
                        // left collision
                        if (oldPosition.X  < mapBlockPosition.X + mapBlockDimensions.X && mapBlockPosition.X < oldPosition.X)
                        {
                            newPosition.X = mapBlockPosition.X + mapBlockDimensions.X;
                        }
                        else // right collision
                        {
                            newPosition.X = mapBlockPosition.X - oldDimensions.X;
                        }
                        newVelocity.X = 0;
                        newAcceleration.X = 0;
                    }
                    else // Hope this never occurs
                    {
                        position_.Position -= velocity_; // back up
                        newVelocity = new Vector2(0, 0);
                        newAcceleration = new Vector2(0, 0);
                    }
                }
            }

            Position = newPosition;
            Velocity = newVelocity;
            Acceleration = newAcceleration;

            return true;
        }
    }
}