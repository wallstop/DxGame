using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Simple
{
    public abstract class SimpleSprite
    {
        protected Vector2 position_ = new Vector2();
        protected Rectangle space_;
        protected Texture2D texture_;
        protected const int BLOCK_WIDTH = 50;
        protected string assetName_;

        protected SimpleSprite(string assetName)
        {
            assetName_ = assetName;
        }

        public void LoadContent(ContentManager contentManager)
        {
            texture_ = contentManager.Load<Texture2D>(assetName_);
            space_ = new Rectangle(0, 0, texture_.Width, texture_.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture_, position_, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
        }

        public static int GetBlockWidth()
        {
            return BLOCK_WIDTH;
        }
    }
}
