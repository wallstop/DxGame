using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.Components.Advanced
{
    // TODO: Still have to test & attach this to the player component
    // TODO: Have this be automagically centered
    // TODO: Have color values be based on "TEAM" (also, introduce concept of teams)
    [Serializable]
    [DataContract]
    public class FloatingHealthIndicator : DrawableComponent, IDisposable
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();

        private const int HEALTH_BAR_PIXEL_HEIGHT = 5;
        private const int HEALTH_BAR_PIXEL_WIDTH = 75;

        /* 
            Serializing colors is nigh impossible. Instead, we can serialize their "packed" values
        */
        [DataMember]
        private uint foregroundColor_;
        [DataMember]
        private uint backgroundColor_;

        [IgnoreDataMember]
        protected Color BackgroundColor {
            get
            {
                var color = new Color
                {
                    A = foregroundColor_.A(),
                    B = foregroundColor_.B(),
                    G = foregroundColor_.C(),
                    R = foregroundColor_.D()
                };
                return color;
            }
            set { foregroundColor_ = value.PackedValue; }
        }

        [IgnoreDataMember]
        protected Color ForegroundColor {
            get
            {
                var color = new Color
                {
                    A = backgroundColor_.A(),
                    B = backgroundColor_.B(),
                    G = backgroundColor_.C(),
                    R = backgroundColor_.D()
                };
                return color;
            }
            set { backgroundColor_ = value.PackedValue; }
        }


        [NonSerialized] [IgnoreDataMember]
        protected Texture2D backgroundTexture_;
        [NonSerialized] [IgnoreDataMember]
        protected Texture2D foregroundTexture_;

        [DataMember]
        protected EntityPropertiesComponent entityProperties_;
        [DataMember]
        protected DxVector2 floatDistance_;

        [DataMember]
        protected PositionalComponent position_;

        public virtual int Health => entityProperties_.Health.CurrentValue;
        public virtual int MaxHealth => entityProperties_.MaxHealth.CurrentValue;
        public virtual double PercentHealthRemaining => (double) Health / MaxHealth;

        protected FloatingHealthIndicator(DxGame game, DxVector2 floatDistance, Color foregroundColor,
            Color backgroundColor, EntityPropertiesComponent properties,
            PositionalComponent position)
            : base(game)
        {
            ValidateFloatDistance(floatDistance);
            Validate.IsNotNullOrDefault(properties, StringUtils.GetFormattedNullOrDefaultMessage(this, properties));
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullOrDefaultMessage(this, position));

            floatDistance_ = floatDistance;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
            entityProperties_ = properties;
            position_ = position;
            DrawPriority = DrawPriority.HUD_LAYER;
        }

        public void Dispose()
        {
            backgroundTexture_?.Dispose();
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
                Validate.IsNotNullOrDefault(position_, StringUtils.GetFormattedNullOrDefaultMessage(typeof(FloatingHealthIndicator), position_));
                Validate.IsNotNullOrDefault(entityProperties_,
                    StringUtils.GetFormattedNullOrDefaultMessage(typeof (FloatingHealthIndicator), entityProperties_));

                return new FloatingHealthIndicator(Main.DxGame.Instance, floatDistance_, foregroundColor_, backgroundColor_, entityProperties_, position_);
            }
        }

        private static void ValidateFloatDistance(DxVector2 floatDistance)
        {
            Validate.IsNotNull(floatDistance,
                $"Cannot intialize {typeof (FloatingHealthIndicator)} with a null floatDistance");
            Validate.IsTrue(floatDistance.Y <= 0,
                $"Cannot use {floatDistance} as a valid FloatDistance for {typeof (FloatingHealthIndicator)} ");
        }

        public override void LoadContent()
        {
            foregroundTexture_ = TextureFactory.TextureForColor(ForegroundColor);
            backgroundTexture_ = TextureFactory.TextureForColor(BackgroundColor);
            base.LoadContent();
        }

        protected static Vector2 DetermineHealthBarOrigin(DxVector2 position,
            DxVector2 floatDistance)
        {
            return (position + floatDistance).ToVector2();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            var origin = DetermineHealthBarOrigin(position_.Position, floatDistance_);
            var foregroundWidth =
                (int) Math.Ceiling(PercentHealthRemaining * HEALTH_BAR_PIXEL_WIDTH);
            spriteBatch.Draw(foregroundTexture_,
                new Rectangle((int) origin.X, (int) origin.Y, foregroundWidth,
                    HEALTH_BAR_PIXEL_HEIGHT), ForegroundColor);
            spriteBatch.Draw(backgroundTexture_,
                new Rectangle((int) origin.X + foregroundWidth, (int) origin.Y,
                    (HEALTH_BAR_PIXEL_WIDTH - foregroundWidth),
                    HEALTH_BAR_PIXEL_HEIGHT), BackgroundColor);
        }
    }
}