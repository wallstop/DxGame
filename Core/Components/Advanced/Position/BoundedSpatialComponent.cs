using System;
using System.Runtime.Serialization;
using DXGame.Core.Messaging;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced.Position
{
    [Serializable]
    [DataContract]
    public class BoundedSpatialComponent : SpatialComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (BoundedSpatialComponent));
        [DataMember] protected DxVector2 xBounds_;
        [DataMember] protected DxVector2 yBounds_;
        public DxVector2 XBounds => xBounds_;
        public DxVector2 YBounds => yBounds_;
        public DxRectangle Bounds => new DxRectangle(XBounds, YBounds);
        /**
        <summary>
            The Position property on a BoundedSpatialComponent
        </summary>
        */

        [IgnoreDataMember]
        public override DxVector2 Position
        {
            get { return position_; }
            set
            {
                float width = dimensions_.X;
                float height = dimensions_.Y;
                float x = MathHelper.Clamp(value.X, xBounds_.X, xBounds_.Y - width);
                float y = MathHelper.Clamp(value.Y, yBounds_.X, yBounds_.Y - height);
                var newPosition = new DxVector2(x, y);
                position_ = newPosition;
                if (newPosition != value)
                {
                    Parent.BroadcastMessage(new CollisionMessage(value - newPosition));
                }
            }
        }

        public BoundedSpatialComponent(DxGame game)
            : base(game)
        {
        }

        public BoundedSpatialComponent WithXMin(float xMin)
        {
            xBounds_ = new DxVector2(xMin, xBounds_.Y);
            return this;
        }

        public BoundedSpatialComponent WithXMax(float xMax)
        {
            xBounds_ = new DxVector2(xBounds_.X, xMax);
            return this;
        }

        public BoundedSpatialComponent WithYMin(float yMin)
        {
            yBounds_ = new DxVector2(yMin, yBounds_.Y);
            return this;
        }

        public BoundedSpatialComponent WithYMax(float yMax)
        {
            yBounds_ = new DxVector2(yBounds_.X, yMax);
            return this;
        }

        public BoundedSpatialComponent WithXBounds(DxVector2 xBounds)
        {
            Validate.IsTrue(xBounds.X <= xBounds.Y);
            xBounds_ = xBounds;
            return this;
        }

        public BoundedSpatialComponent WithYBounds(DxVector2 yBounds)
        {
            Validate.IsTrue(yBounds.X <= yBounds.Y);
            yBounds_ = yBounds;
            return this;
        }

        public BoundedSpatialComponent WithBounds(DxRectangle bounds)
        {
            // To ensure that we don't have any weird Rectangles with negative heights / widths, take the Max/Min

            Validate.IsTrue(bounds.Width >= 0);
            Validate.IsTrue(bounds.Height >= 0);
            yBounds_ = new DxVector2(bounds.Y, bounds.Y + bounds.Height);
            xBounds_ = new DxVector2(bounds.X, bounds.X + bounds.Width);
            return this;
        }
    }
}