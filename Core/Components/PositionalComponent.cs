using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public class PositionalComponent : Component
    {
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
                if ((value.X > bounds_.X) && (value.X < (bounds_.X + Math.Abs(bounds_.Width))))
                {
                    position_.X = value.X;
                }
                else if (value.X <= bounds_.X)
                {
                    position_.X = bounds_.X;
                }
                else
                {
                    position_.X = bounds_.X + Math.Abs(bounds_.Width);
                }

                if ((value.Y > bounds_.Y) && (value.Y < (bounds_.Y + Math.Abs(bounds_.Height))))
                {
                    position_.Y = value.Y;
                }
                else if (value.Y <= bounds_.Y)
                {
                    position_.Y = bounds_.Y;
                }
                else
                {
                    position_.Y = bounds_.Y + Math.Abs(bounds_.Height);
                }
            }
        }

        public PositionalComponent WithBounds(Rectangle bounds)
        {
            Debug.Assert(bounds != null, "PositionalComponent cannot have null boundary");
            bounds_ = bounds;
            return this;
        }

        public PositionalComponent WithPosition(float x, float y)
        {
            position_.X = x;
            position_.Y = y;
            return this;
        }

        public PositionalComponent WithPosition(Vector2 position)
        {
            position_ = position;
            return this;
        }
    }
}