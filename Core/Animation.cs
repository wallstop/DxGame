using System;
using System.Diagnostics;
using DXGame.Core.Components.Advanced;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core
{
    class Animation
    {
        private String assetName_;
        private Texture2D spriteSheet_;
        private int currentFrame_;
        private int totalFrames_;
        protected PositionalComponent position_;

        public Animation(String spriteSheet, int totalFrames = 1)
        {
            assetName_ = spriteSheet;
            totalFrames_ = totalFrames;
        }

        public Animation WithPosition(PositionalComponent position)
        {
            Debug.Assert(position != null, "Sprite position cannot be null on assignment");
            position_ = position;
            return this;
        }

        public string AssetName
        {
            get { return assetName_; }
            set { assetName_ = value; }
        }

        //TODO: make this actually handle spritesheets and animations
        // TOOD: Pass in GameTime
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet_, position_.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0);
        }

        public void Reset()
        {
            currentFrame_ = 0;
        }

        public bool LoadContent(ContentManager contentManager)
        {
            spriteSheet_ = contentManager.Load<Texture2D>(assetName_);
            return true;
        }
    }
}
