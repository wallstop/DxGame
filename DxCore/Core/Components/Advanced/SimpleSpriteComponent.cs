using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class SimpleSpriteComponent : DrawableComponent
    {
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

        private SimpleSpriteComponent(string asset, PositionalComponent position, DxRectangle boundingBox)
        {
            Validate.IsNotNullOrDefault(asset, this.GetFormattedNullOrDefaultMessage(nameof(asset)));
            assetName_ = asset;
            Validate.IsNotNullOrDefault(position, this.GetFormattedNullOrDefaultMessage(position));
            position_ = position;
            boundingBox_ = boundingBox;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(texture_, position_.Position.ToVector2(), null, Color.White, 0.0f, Vector2.Zero, 1.0f,
                SpriteEffects.None, 0);
        }

        public override void Initialize()
        {
            Validate.IsNotNull(DxGame.Instance.Content, this.GetFormattedNullOrDefaultMessage(DxGame.Instance.Content));
            texture_ = DxGame.Instance.Content.Load<Texture2D>(assetName_);
            // Assign boundingBox to be the shape of the texture only if it hasn't been custom-set
            // TODO: Change to an isLoaded bool flag / state
            if(Check.IsNullOrDefault(boundingBox_))
            {
                boundingBox_ = new DxRectangle(0, 0, texture_.Width, texture_.Height);
            }
        }

        public static SimpleSpriteComponentBuilder Builder()
        {
            return new SimpleSpriteComponentBuilder();
        }

        public class SimpleSpriteComponentBuilder : IBuilder<SimpleSpriteComponent>
        {
            private string asset_;
            private DxRectangle boundingBox_;
            private PositionalComponent position_;

            public SimpleSpriteComponent Build()
            {
                return new SimpleSpriteComponent(asset_, position_, boundingBox_);
            }

            public SimpleSpriteComponentBuilder WithAsset(string asset)
            {
                asset_ = asset;
                return this;
            }

            public SimpleSpriteComponentBuilder WithPosition(PositionalComponent position)
            {
                position_ = position;
                return this;
            }

            public SimpleSpriteComponentBuilder WithBoundingBox(DxRectangle boundingBox)
            {
                boundingBox_ = boundingBox;
                return this;
            }
        }
    }
}