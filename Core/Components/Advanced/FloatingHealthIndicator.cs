﻿using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced
{
    // TODO: Still have to test & attach this to the player component
    // TODO: Have this be automagically centered
    // TODO: Have color values be based on "TEAM" (also, introduce concept of teams)
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class FloatingHealthIndicator : DrawableComponent, IDisposable
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private const int HEALTH_BAR_PIXEL_HEIGHT = 5;
        private const int HEALTH_BAR_PIXEL_WIDTH = 75;

        [ProtoMember(1)]
        [DataMember]
        protected DxColor BackgroundColor { get; set; }

        [ProtoMember(2)]
        [DataMember]
        protected DxColor ForegroundColor { get; set; }

        [NonSerialized] [IgnoreDataMember] protected Texture2D backgroundTexture_;
        [NonSerialized] [IgnoreDataMember] protected Texture2D foregroundTexture_;

        [ProtoMember(3)] [DataMember] protected EntityPropertiesComponent entityProperties_;
        [ProtoMember(4)] [DataMember] protected DxVector2 floatDistance_;

        [ProtoMember(5)] [DataMember] protected PositionalComponent position_;

        public virtual int Health => entityProperties_.EntityProperties.Health.CurrentValue;
        public virtual int MaxHealth => entityProperties_.EntityProperties.MaxHealth.CurrentValue;
        public virtual double PercentHealthRemaining => (double) Health / MaxHealth;

        protected FloatingHealthIndicator(DxVector2 floatDistance, Color foregroundColor, Color backgroundColor,
            EntityPropertiesComponent properties, PositionalComponent position)
        {
            ValidateFloatDistance(floatDistance);
            Validate.IsNotNullOrDefault(properties, StringUtils.GetFormattedNullOrDefaultMessage(this, properties));
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullOrDefaultMessage(this, position));

            floatDistance_ = floatDistance;
            ForegroundColor = new DxColor(foregroundColor);
            BackgroundColor = new DxColor(backgroundColor);
            entityProperties_ = properties;
            position_ = position;
            DrawPriority = DrawPriority.HUD_LAYER;
        }

        public override void Dispose()
        {
            backgroundTexture_?.Dispose();
            base.Dispose();
        }

        public static FloatingHealthIndicatorBuilder Builder()
        {
            return new FloatingHealthIndicatorBuilder();
        }

        public class FloatingHealthIndicatorBuilder : IBuilder<FloatingHealthIndicator>
        {
            private DxVector2 floatDistance_ = new DxVector2(-10, -10);
            private Color foregroundColor_ = Color.IndianRed;
            private Color backgroundColor_ = Color.DarkSlateGray;
            private EntityPropertiesComponent entityProperties_;
            private PositionalComponent position_;

            public FloatingHealthIndicatorBuilder WithEntityProperties(EntityPropertiesComponent properties)
            {
                entityProperties_ = properties;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithBackgroundColor(Color backgroundColor)
            {
                backgroundColor_ = backgroundColor;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithForegroundColor(Color foregroundColor)
            {
                foregroundColor_ = foregroundColor;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithPosition(PositionalComponent position)
            {
                position_ = position;
                return this;
            }

            public FloatingHealthIndicatorBuilder WithFloatDistance(DxVector2 floatDistance)
            {
                floatDistance_ = floatDistance;
                return this;
            }

            public FloatingHealthIndicator Build()
            {
                Validate.IsNotNullOrDefault(position_,
                    StringUtils.GetFormattedNullOrDefaultMessage(typeof(FloatingHealthIndicator), position_));
                Validate.IsNotNullOrDefault(entityProperties_,
                    StringUtils.GetFormattedNullOrDefaultMessage(typeof(FloatingHealthIndicator), entityProperties_));

                return new FloatingHealthIndicator(floatDistance_, foregroundColor_, backgroundColor_, entityProperties_,
                    position_);
            }
        }

        private static void ValidateFloatDistance(DxVector2 floatDistance)
        {
            Validate.IsNotNull(floatDistance,
                $"Cannot intialize {typeof(FloatingHealthIndicator)} with a null floatDistance");
            Validate.IsTrue(floatDistance.Y <= 0,
                $"Cannot use {floatDistance} as a valid FloatDistance for {typeof(FloatingHealthIndicator)} ");
        }

        public override void LoadContent()
        {
            foregroundTexture_ = TextureFactory.TextureForColor(ForegroundColor.Color);
            backgroundTexture_ = TextureFactory.TextureForColor(BackgroundColor.Color);
            base.LoadContent();
        }

        protected static Vector2 DetermineHealthBarOrigin(DxVector2 position, DxVector2 floatDistance)
        {
            return (position + floatDistance).ToVector2();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var origin = DetermineHealthBarOrigin(position_.Position, floatDistance_);
            var foregroundWidth = (int) Math.Ceiling(PercentHealthRemaining * HEALTH_BAR_PIXEL_WIDTH);
            spriteBatch.Draw(foregroundTexture_,
                new Rectangle((int) origin.X, (int) origin.Y, foregroundWidth, HEALTH_BAR_PIXEL_HEIGHT),
                ForegroundColor.Color);
            spriteBatch.Draw(backgroundTexture_,
                new Rectangle((int) origin.X + foregroundWidth, (int) origin.Y, HEALTH_BAR_PIXEL_WIDTH - foregroundWidth,
                    HEALTH_BAR_PIXEL_HEIGHT), BackgroundColor.Color);
        }
    }
}