﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Sprite
{
    [Serializable]
    [DataContract]
    public class SimpleSpriteComponent : DrawableComponent, ISpatial
    {
        [DataMember] protected string assetName_;
        [DataMember] protected ISpatial spatial_;
        [NonSerialized] [IgnoreDataMember] protected Texture2D texture_;

        [IgnoreDataMember]
        public string AssetName
        {
            get { return assetName_; }
            set { assetName_ = value; }
        }

        private SimpleSpriteComponent(string asset, ISpatial spatial)
        {
            assetName_ = asset;
            spatial_ = spatial;
        }

        public DxVector2 WorldCoordinates => spatial_.WorldCoordinates;
        public DxRectangle Space => spatial_.Space;

        public static SimpleSpriteComponentBuilder Builder()
        {
            return new SimpleSpriteComponentBuilder();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(texture_, spatial_.Space.Rectangle, Color.White);
        }

        public override void Initialize()
        {
            Validate.Hard.IsNotNull(DxGame.Instance.Content,
                this.GetFormattedNullOrDefaultMessage(DxGame.Instance.Content));
            texture_ = DxGame.Instance.Content.Load<Texture2D>(assetName_);
        }

        public class SimpleSpriteComponentBuilder : IBuilder<SimpleSpriteComponent>
        {
            private string asset_;
            private ISpatial spatial_;

            public SimpleSpriteComponent Build()
            {
                Validate.Hard.IsNotNullOrDefault(asset_);
                Validate.Hard.IsNotNullOrDefault(spatial_);
                return new SimpleSpriteComponent(asset_, spatial_);
            }

            public SimpleSpriteComponentBuilder WithAsset(string asset)
            {
                asset_ = asset;
                return this;
            }

            public SimpleSpriteComponentBuilder WithComponent(SimpleSpriteComponent alreadyExists)
            {
                Validate.Hard.IsNotNull(alreadyExists);
                asset_ = alreadyExists.assetName_;
                spatial_ = alreadyExists.spatial_;
                return this;
            }

            public SimpleSpriteComponentBuilder WithSpatial(ISpatial spatial)
            {
                spatial_ = spatial;
                return this;
            }
        }
    }
}