using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced.Position
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
        public DxVector2 XBounds { get; }

        [DataMember]
        public DxVector2 YBounds { get; }

        public DxRectangle Bounds => new DxRectangle(XBounds.X, YBounds.Y, XBounds.Y - XBounds.X, YBounds.Y - YBounds.X);

        [IgnoreDataMember]
        public override DxVector2 Position
        {
            get { return position_; }
            set
            {
                float width = Dimensions.X;
                float height = Dimensions.Y;
                float x = MathHelper.Clamp(value.X, XBounds.X, XBounds.Y - width);
                float y = MathHelper.Clamp(value.Y, YBounds.X, YBounds.Y - height);
                var newPosition = new DxVector2(x, y);
                position_ = newPosition;
                if (newPosition != value)
                {
                    Parent.BroadcastMessage(new CollisionMessage(value - newPosition));
                }
            }
        }

        protected BoundedSpatialComponent(DxGame game, DxVector2 position, DxVector2 dimensions, DxVector2 xBounds,
            DxVector2 yBounds)
            : base(game, position, dimensions)
        {
            XBounds = xBounds;
            YBounds = yBounds;
        }

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
                var game = DxGame.Instance;
                return new BoundedSpatialComponent(game, position_, dimensions_, xBounds_, yBounds_);
            }
        }
    }
}