using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Physics;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class SimpleSpriteComponent : DrawableComponent
    {
        [DataMember] protected string assetName_;
        [DataMember] protected PhysicsComponent position_;
        [NonSerialized] [IgnoreDataMember] protected Texture2D texture_;

        [IgnoreDataMember]
        public string AssetName
        {
            get { return assetName_; }
            set { assetName_ = value; }
        }

        private SimpleSpriteComponent(string asset, PhysicsComponent position)
        {
            Validate.Hard.IsNotNullOrDefault(asset, this.GetFormattedNullOrDefaultMessage(nameof(asset)));
            assetName_ = asset;
            Validate.Hard.IsNotNullOrDefault(position, this.GetFormattedNullOrDefaultMessage(position));
            position_ = position;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(texture_, position_.Space.Rectangle, Color.White);
        }

        public override void Initialize()
        {
            Validate.Hard.IsNotNull(DxGame.Instance.Content, this.GetFormattedNullOrDefaultMessage(DxGame.Instance.Content));
            texture_ = DxGame.Instance.Content.Load<Texture2D>(assetName_);
        }

        public static SimpleSpriteComponentBuilder Builder()
        {
            return new SimpleSpriteComponentBuilder();
        }

        public class SimpleSpriteComponentBuilder : IBuilder<SimpleSpriteComponent>
        {
            private string asset_;
            private PhysicsComponent position_;

            public SimpleSpriteComponent Build()
            {
                return new SimpleSpriteComponent(asset_, position_);
            }

            public SimpleSpriteComponentBuilder WithAsset(string asset)
            {
                asset_ = asset;
                return this;
            }

            public SimpleSpriteComponentBuilder WithPosition(PhysicsComponent position)
            {
                position_ = position;
                return this;
            }
        }
    }
}