using DXGame.Core.Utils;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class BoundedSpatialComponent : SpatialComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (BoundedSpatialComponent));

        protected Vector2 xBounds_;
        protected Vector2 yBounds_;

        public BoundedSpatialComponent(GameObject parent = null)
            : base(parent)
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

        public override Vector2 Position
        {
            get { return position_; }
            set
            {
                float width = widthHeight_.X;
                float height = widthHeight_.Y;
                float x = MathUtils.Constrain(value.X, xBounds_.X, xBounds_.Y - width);
                float y = MathUtils.Constrain(value.Y, yBounds_.X, yBounds_.Y - height);
                position_ = new Vector2(x, y);
            }
        }
    }
}