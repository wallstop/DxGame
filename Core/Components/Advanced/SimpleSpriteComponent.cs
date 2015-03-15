using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class SimpleSpriteComponent : DrawableComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SimpleSpriteComponent));

        protected string assetName_;
        protected Rectangle boundingBox_;
        protected PositionalComponent position_;
        protected Texture2D texture_;

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

        public SimpleSpriteComponent(DxGame game)
            : base(game)
        {
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

        public override void Draw(GameTime gameTime)
        {
            spriteBatch_.Draw(texture_, position_.Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0);
            base.Draw(gameTime);
        }

        public override void Initialize()
        {
            Debug.Assert(Game.Content != null, "ContentManager cannot be null during Initialize");
            texture_ = Game.Content.Load<Texture2D>(assetName_);
            // Assign boundingBox to be the shape of the texture only if it hasn't been custom-set
            // TODO: Change to an isLoaded bool flag / state
            if (GenericUtils.IsNullOrDefault(boundingBox_))
            {
                boundingBox_ = new Rectangle(0, 0, texture_.Width, texture_.Height);
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }
    }
}