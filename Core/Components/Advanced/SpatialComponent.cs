using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>
        A SpatialComponent is a PositionalComponent that has knowledge of the Rectangle of space that it consumes.

    </summary>
    */

    [Serializable]
    [DataContract]
    public class SpatialComponent : PositionalComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SpatialComponent));

        [DataMember] protected DxVector2 dimensions_;

        /**
        <summary>
            The bounding box
        </summary>
        */

        public virtual DxRectangle Space
        {
            get { return VectorUtils.RectangleFrom(position_, dimensions_); }
        }

        public virtual float Width
        {
            get { return dimensions_.X; }
        }

        public virtual float Height
        {
            get { return dimensions_.Y; }
        }

        public virtual DxVector2 Dimensions
        {
            get { return dimensions_; }
        }

        public SpatialComponent(DxGame game)
            : base(game)
        {
        }

        public virtual SpatialComponent WithDimensions(DxVector2 dimensions)
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

        public virtual DxVector2 Center
        {
            get { return position_ + dimensions_ / 2.0f; }
        }
    }
}