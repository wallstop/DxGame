using System.Diagnostics;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class SpatialComponent : Component
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SpatialComponent));

        protected Vector2 widthHeight_;
        protected PositionalComponent position_;

        public virtual Rectangle Space
        {
            get
            {
                return new Rectangle((int) position_.Position.X, (int) position_.Position.Y, (int) widthHeight_.X,
                    (int) widthHeight_.Y);
            }
        }

        public virtual Vector2 Position
        {
            get { return position_.Position; }
            set { position_.Position = value; }
        }

        public virtual float Width
        {
            get { return widthHeight_.X; }
        }

        public virtual float Height
        {
            get { return widthHeight_.Y; }
        }

        public SpatialComponent(GameObject parent = null)
            : base(parent)
        {
        }

        public virtual SpatialComponent WithPosition(PositionalComponent position)
        {
            Debug.Assert(position != null, "SpatialComponent cannot be constructed with null position");
            position_ = position;
            return this;
        }

        public virtual SpatialComponent WithWidthAndHeight(Vector2 widthAndHeight)
        {
            Debug.Assert(widthAndHeight != null, "SpatialComponent cannot be constructed with null width and height");
            widthHeight_ = widthAndHeight;
            return this;
        }
    }
}