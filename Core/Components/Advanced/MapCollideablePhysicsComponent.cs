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
            Vector2 currentPosition = position_.Position;
            Vector2 currentDimensions = Dimensions;
            Rectangle currentSpace = Space;

            var map = GameState.Model<MapModel>();
            IEnumerable<SpatialComponent> mapTiles =
                map.SpatialsInRange(MapQueryRegion);

            Vector2 offset = new Vector2();
            foreach (SpatialComponent spatial in mapTiles)
            {
                Rectangle space = spatial.Space;
                if (currentSpace.Intersects(space))
                {
                    // TODO: Fix this (it suckkssss, some of the math is wrong)
                    // Need to base collision off of trajectory.
                    var tempCurrentPosition = currentPosition + offset;
                    // Left side collision
                    if (space.X < tempCurrentPosition.X && space.X + space.Width >= tempCurrentPosition.X)
                    {
                        offset.X = space.X + space.Width - tempCurrentPosition.X;
                    }

                    // Top side collision
                    if (space.Y < tempCurrentPosition.Y && space.Y + space.Height >= tempCurrentPosition.Y)
                    {
                        offset.Y = space.Y + space.Height - tempCurrentPosition.Y;
                    }

                    // Right side collision
                    if (space.X > tempCurrentPosition.X && space.X <= tempCurrentPosition.X + currentDimensions.X)
                    {
                        offset.X = currentDimensions.X - (space.X - tempCurrentPosition.X);
                    }

                    // Bottom collision
                    if (space.Y > tempCurrentPosition.Y && space.Y <= tempCurrentPosition.Y + currentDimensions.Y)
                    {
                        offset.Y = space.Y - tempCurrentPosition.Y - currentDimensions.Y;
                    }
                }
            }

            if (offset.X != 0.0f)
            {
                velocity_.X = 0.0f;
                acceleration_.X = 0.0f;
            }
            if (offset.Y != 0.0f)
            {
                velocity_.Y = 0.0f;
                acceleration_.Y = 0.0f;
            }

            position_.Position = currentPosition + offset;

            return true;
        }
    }
}