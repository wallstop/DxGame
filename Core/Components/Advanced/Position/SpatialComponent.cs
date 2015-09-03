using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using NLog;

namespace DXGame.Core.Components.Advanced.Position
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
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        [DataMember] protected DxVector2 dimensions_;
        /**
        <summary>
            The bounding box
        </summary>
        */

        public virtual DxRectangle Space => new DxRectangle(Position.X, Position.Y, Dimensions.X, Dimensions.Y);// VectorUtils.RectangleFrom(position_, dimensions_);
        public virtual float Width => dimensions_.X;
        public virtual float Height => dimensions_.Y;
        public virtual DxVector2 Dimensions => dimensions_;
        /**
        <summary>
            The center position that the SpatialComponent occupies. This is equivalane to 
            new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
        </summary>
        */

        public virtual DxVector2 Center => position_ + dimensions_ / 2.0f;

        public SpatialComponent(DxGame game)
            : base(game)
        {
        }

        public virtual SpatialComponent WithDimensions(DxVector2 dimensions)
        {
            Validate.IsNotNullOrDefault(dimensions,
                $"Cannot initialize {GetType()} with null/default {nameof(dimensions)}");
            dimensions_ = dimensions;
            return this;
        }
    }
}