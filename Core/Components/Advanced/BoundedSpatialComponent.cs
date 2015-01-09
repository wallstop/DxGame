using System;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>

    </summary>
    */

    public class BoundedSpatialComponent : SpatialComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (BoundedSpatialComponent));

        protected Vector2 xBounds_;
        protected Vector2 yBounds_;

        public BoundedSpatialComponent(DxGame game)
            : base(game)
        {
        }

        public BoundedSpatialComponent WithXMin(float xMin)
        {
            xBounds_ = new Vector2(xMin, xBounds_.Y);
            return this;
        }

        public BoundedSpatialComponent WithXMax(float xMax)
        {
            xBounds_ = new Vector2(xBounds_.X, xMax);
            return this;
        }

        public BoundedSpatialComponent WithYMin(float yMin)
        {
            yBounds_ = new Vector2(yMin, yBounds_.Y);
            return this;
        }

        public BoundedSpatialComponent WithYMax(float yMax)
        {
            yBounds_ = new Vector2(yBounds_.X, yMax);
            return this;
        }

        public BoundedSpatialComponent WithXBounds(Vector2 xBounds)
        {
            xBounds_ = xBounds;
            return this;
        }

        public BoundedSpatialComponent WithYBounds(Vector2 yBounds)
        {
            yBounds_ = yBounds;
            return this;
        }

        // TODO: Create a Rectangle class based around floats instead of ints
        public BoundedSpatialComponent WithBounds(Rectangle bounds)
        {
            // To ensure that we don't have any weird Rectangles with negative heights / widths, take the Max/Min

            // TODO: Remove the Min/Max checks, replace with an assert or similar
            yBounds_ = new Vector2(Math.Min(bounds.Y, bounds.Y + bounds.Height),
                Math.Max(bounds.Y, bounds.Y + bounds.Height));
            xBounds_ = new
                Vector2(Math.Min(bounds.X, bounds.X + bounds.Width), Math.Max(bounds.X, bounds.X + bounds.Width));
            return this;
        }

        /**
        <summary>
            The Position property on a BoundedSpatialComponent
        </summary>
        */
        public override Vector2 Position
        {
            get { return position_; }
            set
            {
                float width = dimensions_.X;
                float height = dimensions_.Y;
                float x = MathUtils.Constrain(value.X, xBounds_.X, xBounds_.Y - width);
                float y = MathUtils.Constrain(value.Y, yBounds_.X, yBounds_.Y - height);
                grounded_ = (y != value.Y);
                position_ = new Vector2(x, y);
            }
        }
    }
}