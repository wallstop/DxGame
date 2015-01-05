using System.Diagnostics;
using DXGame.Core.Components.Basic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced
{
    public class SimpleSpriteComponent : DrawableComponent
    {
        protected string assetName_;
        protected Rectangle boundingBox_;
        protected PositionalComponent position_;
        protected Texture2D texture_;

        public SimpleSpriteComponent(string assetName = "", PositionalComponent position = null,
            GameObject parent = null)
            : base(parent)
        {
            assetName_ = assetName;
            position_ = position;
        }

        public Rectangle BoundingBox
        {
            get { return boundingBox_; }
            set { boundingBox_ = value; }
        }

        public string AssetName
        {
            get { return assetName_; }
            set { assetName_ = value; }
        }

        public SimpleSpriteComponent WithAsset(string assetName)
        {
            Debug.Assert(assetName != null, "AssetName cannot be null on assignment");
            assetName_ = assetName;
            return this;
        }

        public SimpleSpriteComponent WithPosition(PositionalComponent position)
        {
            Debug.Assert(position != null, "Sprite position cannot be null on assignment");
            position_ = position;
            return this;
        }

        public SimpleSpriteComponent WithBoundingBox(Rectangle boundingBox)
        {
            Debug.Assert(boundingBox != null, "Bounding box cannot be null on assignment");
            boundingBox_ = boundingBox;
            return this;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture_, position_.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0);
            return true;
        }

        public override bool LoadContent(ContentManager contentManager)
        {
            Debug.Assert(contentManager != null, "ContentManager cannot be null during LoadContent");
            texture_ = contentManager.Load<Texture2D>(assetName_);
            // Assign boundingBox to be the shape of the texture only if it hasn't been custom-set
            if (boundingBox_ == default(Rectangle))
            {
                boundingBox_ = new Rectangle(0, 0, texture_.Width, texture_.Height);
            }
            return true;
        }
    }
}