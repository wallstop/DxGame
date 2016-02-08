using System;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using NLog;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced.Position
{
    /**
        <summary>
            A SpatialComponent is a PositionalComponent that has knowledge of the Rectangle of space that it consumes.
        </summary>
    */

    [Serializable]
    [DataContract]
    [ProtoContract]
    public class SpatialComponent : PositionalComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        [ProtoMember(1)]
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

        public class SpatialComponentBuilder : PositionalComponentBuilder
        {
            protected DxVector2 dimensions_;

            public SpatialComponentBuilder WithDimensions(DxVector2 dimensions)
            {
                dimensions_ = dimensions;
                return this;
            }

            public override PositionalComponent Build()
            {
                return new SpatialComponent(position_, dimensions_);
            }
        }
    }
}