using System.Diagnostics;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    public class SpatialComponent : PositionalComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SpatialComponent));

        protected Vector2 widthHeight_;

        public virtual Rectangle Space
        {
            get
            {
                return new Rectangle((int) Position.X, (int) Position.Y, (int) widthHeight_.X,
                    (int) widthHeight_.Y);
            }
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
            : base(parent: parent)
        {
        }

        public virtual SpatialComponent WithWidthAndHeight(Vector2 widthAndHeight)
        {
            Debug.Assert(widthAndHeight != null, "SpatialComponent cannot be constructed with null width and height");
            widthHeight_ = widthAndHeight;
            return this;
        }

        public virtual Vector2 PositionShift
        {
            get { return new Vector2(position_.X + widthHeight_.X, position_.Y + widthHeight_.Y/2);}
        }
    }
}