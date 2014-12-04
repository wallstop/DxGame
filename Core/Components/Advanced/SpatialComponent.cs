using System.Diagnostics;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>
        A SpatialComponent is a PositionalComponent that has knowledge of the Rectangle of space that it consumes.

    </summary>
    */

    public class SpatialComponent : PositionalComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SpatialComponent));

        protected Vector2 dimensions_;

        /**
        <summary>
            The bounding box
        </summary>
        */
        public virtual Rectangle Space
        {
            get
            {
                return new Rectangle((int) Position.X, (int) Position.Y, (int) dimensions_.X,
                    (int) dimensions_.Y);
            }
        }

        public virtual float Width
        {
            get { return dimensions_.X; }
        }

        public virtual float Height
        {
            get { return dimensions_.Y; }
        }

        public SpatialComponent(GameObject parent = null)
            : base(parent: parent)
        {
        }

        public virtual SpatialComponent WithDimensions(Vector2 dimensions)
        {
            Debug.Assert(dimensions != null, "SpatialComponent cannot be constructed with null dimensions");
            dimensions_ = dimensions;
            return this;
        }

        /**
        <summary>
            The center position that the SpatialComponent occupies. This is equivalane to 
            new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
        </summary>
        */

        public virtual Vector2 Center
        {
            get { return position_ + dimensions_ / 2.0f; }
        }
    }
}