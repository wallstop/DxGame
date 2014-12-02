using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public class PositionalComponent : Component
    {
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

        public PositionalComponent WithCoordinates(float x, float y)
        {
            position_.X = x;
            position_.Y = y;
            return this;
        }

        public virtual Vector2 Position
        {
            get { return position_; }
            set { position_ = value; }
        }
    }
}