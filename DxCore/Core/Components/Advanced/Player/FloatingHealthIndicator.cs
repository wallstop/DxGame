﻿using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Advanced.Properties;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Advanced.Player
{
    // TODO: Still have to test & attach this to the player component
    // TODO: Have this be automagically centered
    // TODO: Have color values be based on "TEAM" (also, introduce concept of teams)
    [Serializable]
    [DataContract]
    public class FloatingHealthIndicator : DrawableComponent
    {
        private const int HEALTH_BAR_PIXEL_HEIGHT = 5;
        private const int HEALTH_BAR_PIXEL_WIDTH = 75;

        [NonSerialized] [IgnoreDataMember] protected Texture2D backgroundTexture_;

        [DataMember] protected EntityPropertiesComponent entityProperties_;
        [DataMember] protected DxVector2 floatDistance_;
        [NonSerialized] [IgnoreDataMember] protected Texture2D foregroundTexture_;

        [DataMember] protected IPositional position_;

        public virtual int Health => entityProperties_.EntityProperties.Health.CurrentValue;
        public virtual int MaxHealth => entityProperties_.EntityProperties.MaxHealth.CurrentValue;
        public virtual double PercentHealthRemaining => (double) Health / MaxHealth;

        [DataMember]
        protected DxColor BackgroundColor { get; set; }

        [DataMember]
        protected DxColor ForegroundColor { get; set; }

        protected FloatingHealthIndicator(DxVector2 floatDistance, Color foregroundColor, Color backgroundColor,
            EntityPropertiesComponent properties, IPositional position)
        {
            ValidateFloatDistance(floatDistance);
            Validate.Hard.IsNotNullOrDefault(properties, this.GetFormattedNullOrDefaultMessage(properties));
            Validate.Hard.IsNotNullOrDefault(position, this.GetFormattedNullOrDefaultMessage(position));

            floatDistance_ = floatDistance;
            ForegroundColor = new DxColor(foregroundColor);
            BackgroundColor = new DxColor(backgroundColor);
            entityProperties_ = properties;
            position_ = position;
            DrawPriority = DrawPriority.HudLayer;
        }

        public static FloatingHealthIndicatorBuilder Builder()
        {
            return new FloatingHealthIndicatorBuilder();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            Vector2 origin = DetermineHealthBarOrigin(position_.WorldCoordinates, floatDistance_);
            int foregroundWidth = (int) Math.Ceiling(PercentHealthRemaining * HEALTH_BAR_PIXEL_WIDTH);
            spriteBatch.Draw(foregroundTexture_,
                new Rectangle((int) origin.X, (int) origin.Y, foregroundWidth, HEALTH_BAR_PIXEL_HEIGHT),
                ForegroundColor.Color);
            spriteBatch.Draw(backgroundTexture_,
                new Rectangle((int) origin.X + foregroundWidth, (int) origin.Y, HEALTH_BAR_PIXEL_WIDTH - foregroundWidth,
                    HEALTH_BAR_PIXEL_HEIGHT), BackgroundColor.Color);
        }

        public override void LoadContent()
        {
            foregroundTexture_ = TextureFactory.TextureForColor(ForegroundColor.Color);
            backgroundTexture_ = TextureFactory.TextureForColor(BackgroundColor.Color);
            base.LoadContent();
        }

        public override void Remove()
        {
            backgroundTexture_?.Dispose();
            base.Remove();
        }

        protected static Vector2 DetermineHealthBarOrigin(DxVector2 position, DxVector2 floatDistance)
        {
            return (position + floatDistance).Vector2;
        }

        private static void ValidateFloatDistance(DxVector2 floatDistance)
        {
            Validate.Hard.IsNotNull(floatDistance,
                () => $"Cannot intialize {typeof(FloatingHealthIndicator)} with a null floatDistance");
            Validate.Hard.IsTrue(floatDistance.Y <= 0,
                () => $"Cannot use {floatDistance} as a valid FloatDistance for {typeof(FloatingHealthIndicator)} ");
        }

        public class FloatingHealthIndicatorBuilder : IBuilder<FloatingHealthIndicator>
        {
            private Color backgroundColor_ = Color.DarkSlateGray;
            private EntityPropertiesComponent entityProperties_;
            private DxVector2 floatDistance_ = new DxVector2(-10, -10);
            private Color foregroundColor_ = Color.IndianRed;
            private IPositional position_;

            public FloatingHealthIndicator Build()
            {
                Validate.Hard.IsNotNullOrDefault(position_,
                    StringUtils.GetFormattedNullOrDefaultMessage(typeof(FloatingHealthIndicator), position_));
                Validate.Hard.IsNotNullOrDefault(entityProperties_,
                    StringUtils.GetFormattedNullOrDefaultMessage(typeof(FloatingHealthIndicator), entityProperties_));

                return new FloatingHealthIndicator(floatDistance_, foregroundColor_, backgroundColor_, entityProperties_,
                    position_);
            }

            public FloatingHealthIndicatorBuilder WithBackgroundColor(Color backgroundColor)
            {
                backgroundColor_ = backgroundColor;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithEntityProperties(EntityPropertiesComponent properties)
            {
                entityProperties_ = properties;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithFloatDistance(DxVector2 floatDistance)
            {
                floatDistance_ = floatDistance;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithForegroundColor(Color foregroundColor)
            {
                foregroundColor_ = foregroundColor;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithPosition(IPositional position)
            {
                position_ = position;
                return this;
            }
        }
    }
}