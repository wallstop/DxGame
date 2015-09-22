using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class SimpleSpriteComponent : DrawableComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        [DataMember] protected string assetName_;
        [DataMember] protected DxRectangle boundingBox_;
        [DataMember] protected PositionalComponent position_;
        [NonSerialized] [IgnoreDataMember] protected Texture2D texture_;

        [IgnoreDataMember]
        public DxRectangle BoundingBox
        {
            get { return boundingBox_; }
            set { boundingBox_ = value; }
        }

        [IgnoreDataMember]
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
            Validate.IsNotNullOrDefault(assetName,
                $"Cannot initialize {GetType()} with a null/default {nameof(assetName)}");
            assetName_ = assetName;
            return this;
        }

        public SimpleSpriteComponent WithPosition(PositionalComponent position)
        {
            Validate.IsNotNullOrDefault(position,
                $"Cannot initialize {GetType()} with a null/default {nameof(position)}");
            position_ = position;
            return this;
        }

        public SimpleSpriteComponent WithBoundingBox(DxRectangle boundingBox)
        {
            Validate.IsNotNullOrDefault(boundingBox,
                $"Cannot initialize {GetType()} with a null/default {nameof(boundingBox)}");
            boundingBox_ = boundingBox;
            return this;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(texture_, position_.Position.ToVector2(), null, Color.White, 0.0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0);
        }

        public override void Initialize()
        {
            Validate.IsNotNull(DxGame.Content, StringUtils.GetFormattedNullOrDefaultMessage(this, DxGame.Content));
            texture_ = DxGame.Content.Load<Texture2D>(assetName_);
            // Assign boundingBox to be the shape of the texture only if it hasn't been custom-set
            // TODO: Change to an isLoaded bool flag / state
            if (Check.IsNullOrDefault(boundingBox_))
            {
                boundingBox_ = new DxRectangle(0, 0, texture_.Width, texture_.Height);
            }
            base.Initialize();
        }
    }
}