using System;
using System.Runtime.Serialization;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;

namespace DxCore.Core.Components.Advanced.Position
{
    /**

        <summary>
            A SpatialComponent that is, surprise surprise, bounded! By what? Who knows! Something!
            It's position cannot be set outside of these bounds. This is extremely helpful for 
            binding entities to the map region
        </summary>
    */

    [Serializable]
    [DataContract]
    public class BoundedSpatialComponent : SpatialComponent
    {
        [DataMember]
        public virtual DxRectangle Bounds { get; }

        [IgnoreDataMember]
        public override DxVector2 Position
        {
            get { return position_; }
            set
            {
                float width = Dimensions.X;
                float height = Dimensions.Y;
                DxRectangle bounds = Bounds;
                float x = MathHelper.Clamp(value.X, bounds.X, bounds.X + bounds.Width - width);
                float y = MathHelper.Clamp(value.Y, bounds.Y, bounds.Y + bounds.Height - height);
                var newPosition = new DxVector2(x, y);
                position_ = newPosition;
                if(newPosition != value)
                {
                    CollisionMessage collision = new CollisionMessage(value - newPosition, this) {Target = Parent?.Id};
                    collision.Emit();
                }
            }
        }

        protected BoundedSpatialComponent(DxVector2 position, DxVector2 dimensions, DxVector2 xBounds, DxVector2 yBounds)
            : base(position, dimensions)
        {
            Bounds = new DxRectangle(xBounds.X, xBounds.Y - xBounds.X, yBounds.X, yBounds.Y - yBounds.X);
        }

        /* Useful for subclasses that have their own special bound logic */
        protected BoundedSpatialComponent(DxVector2 position, DxVector2 dimensions) : base(position, dimensions) {}

        public new static BoundedSpatialComponentBuilder Builder()
        {
            return new BoundedSpatialComponentBuilder();
        }

        public class BoundedSpatialComponentBuilder : SpatialComponentBuilder
        {
            protected DxVector2 xBounds_;
            protected DxVector2 yBounds_;

            public BoundedSpatialComponentBuilder WithBounds(DxRectangle bounds)
            {
                xBounds_ = new DxVector2(bounds.X, bounds.Width);
                yBounds_ = new DxVector2(bounds.Y, bounds.Height);
                return this;
            }

            public BoundedSpatialComponentBuilder WithXBounds(DxVector2 xBounds)
            {
                xBounds_ = xBounds;
                return this;
            }

            public BoundedSpatialComponentBuilder WithYBounds(DxVector2 yBounds)
            {
                yBounds_ = yBounds;
                return this;
            }

            public override PositionalComponent Build()
            {
                Validate.IsTrue(xBounds_.Magnitude > 0);
                Validate.IsTrue(yBounds_.Magnitude > 0);
                return new BoundedSpatialComponent(position_, dimensions_, xBounds_, yBounds_);
            }
        }
    }
}