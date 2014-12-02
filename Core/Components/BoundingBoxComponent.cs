using Microsoft.Xna.Framework;

namespace DXGame.Core.Components
{
    public class BoundedBoxComponent : Component
    {
        protected Rectangle boundingBox_;
        protected PositionalComponent position_;

        public BoundedBoxComponent(PositionalComponent position = null, GameObject parent = null)
            : base(parent)
        {
            boundingBox_ = new Rectangle();
        }

        public BoundedBoxComponent(Rectangle boundingBox, GameObject parent = null)
            : base(parent)
        {
            boundingBox_ = boundingBox;
        }

        public Rectangle BoundingBox
        {
            get { return boundingBox_; }
            set { boundingBox_ = value; }
        }

        public BoundedBoxComponent WithWidthAndHeight(int width, int height)
        {
            boundingBox_ = new Rectangle((int) position_.Position.X, (int) position_.Position.Y, width, height);
            return this;
        }

        public BoundedBoxComponent WithPosition(PositionalComponent position)
        {
            position_ = position;
            return this;
        }
    }
}