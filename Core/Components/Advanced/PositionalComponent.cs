using System.Diagnostics;
using DXGame.Core.Utils;
using log4net;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components.Advanced
{
    /**
    <summary>
        PositionalComponent is a component that keeps track of a position in space and time
    */

    public class PositionalComponent : Component
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (PositionalComponent));
        protected Rectangle bounds_;
        protected Vector2 position_;

        public PositionalComponent(float x = 0.0f, float y = 0.0f, GameObject parent = null)
            : base(parent)
        {
            position_.X = x;
            position_.Y = y;
        }

        public PositionalComponent(Vector2 position, GameObject parent = null)
            : base(parent)
        {
            position_ = position;
        }

        public virtual Vector2 Position
        {
            get { return position_; }
            set
            {
                position_.X = MathUtils.Constrain(value.X, bounds_.X, bounds_.X + bounds_.Width);
                position_.Y = MathUtils.Constrain(value.Y, bounds_.Y, bounds_.Y + bounds_.Height);
            }
        }

        public virtual PositionalComponent WithBounds(Rectangle bounds)
        {
            Debug.Assert(bounds != null, "PositionalComponent cannot have null boundary");
            bounds_ = bounds;
            return this;
        }

        public virtual PositionalComponent WithPosition(float x, float y)
        {
            position_.X = x;
            position_.Y = y;
            return this;
        }

        public virtual PositionalComponent WithPosition(Vector2 position)
        {
            position_ = position;
            return this;
        }
    }
}