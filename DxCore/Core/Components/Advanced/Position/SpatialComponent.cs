using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;

namespace DxCore.Core.Components.Advanced.Position
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
        [DataMember]
        public DxVector2 Dimensions { get; }

        /**
            <summary>
                The bounding box that encompasses this component
            </summary>
        */
        public virtual DxRectangle Space => new DxRectangle(Position.X, Position.Y, Dimensions.X, Dimensions.Y);
        public virtual float Width => Dimensions.X;
        public virtual float Height => Dimensions.Y;

        /**
            <summary>
                The center position that the SpatialComponent occupies. This is equivalane to 
                new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
            </summary>
        */
        public virtual DxVector2 Center => position_ + Dimensions / 2.0f;

        protected SpatialComponent(DxVector2 position, DxVector2 dimensions) : base(position)
        {
            Dimensions = dimensions;
        }

        public new static SpatialComponentBuilder Builder()
        {
            return new SpatialComponentBuilder();
        }

        public class SpatialComponentBuilder : IBuilder<SpatialComponent>
        {
            private DxVector2 dimensions_;
            private DxVector2 position_;

            public SpatialComponentBuilder WithPosition(DxVector2 position)
            {
                position_ = position;
                return this;
            }

            public SpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }
            public SpatialComponent Build()
            {
                return new SpatialComponent(position_, dimensions_);
            }
        }
    }
}