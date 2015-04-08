using System;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Advanced.Properties;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Advanced
{
    // TODO: Still have to test & attach this to the player component
    // TODO: Have this be automagically centered
    public class FloatingHealthIndicator : DrawableComponent
    {
        private const int HEALTH_BAR_PIXEL_HEIGHT = 5;
        private const int HEALTH_BAR_PIXEL_WIDTH = 75;
        private static readonly ILog LOG = LogManager.GetLogger(typeof (FloatingHealthIndicator));
        protected Color backgroundColor_;
        protected Texture2D backgroundTexture_;
        protected EntityPropertiesComponent entityProperties_;
        protected DxVector2 floatDistance_;
        protected Color foregroundColor_;
        protected Texture2D foregroundTexture_;
        protected PositionalComponent position_;

        public virtual int Health
        {
            get { return entityProperties_.Health.CurrentValue; }
        }

        public virtual int MaxHealth
        {
            get { return entityProperties_.MaxHealth.CurrentValue; }
        }

        public virtual double PercentHealthRemaining
        {
            get { return (double) Health / MaxHealth; }
        }

        public FloatingHealthIndicator(DxGame game, DxVector2 floatDistance, Color foregroundColor,
            Color backgroundColor, EntityPropertiesComponent properties,
            PositionalComponent position)
            : base(game)
        {
            ValidateFloatDistance(floatDistance);
            GenericUtils.CheckNullOrDefault(properties);
            GenericUtils.CheckNullOrDefault(position);

            floatDistance_ = floatDistance;
            foregroundColor_ = foregroundColor;
            backgroundColor_ = backgroundColor;
            entityProperties_ = properties;
            position_ = position;
            DrawPriority = DrawPriority.HUD_LAYER;
        }

        private static void ValidateFloatDistance(DxVector2 floatDistance)
        {
            GenericUtils.CheckNullOrDefault(floatDistance,
                "Floating Health Indicator cannot be initialized with a null/default floatDistance");
            if (floatDistance.X >= 0)
            {
                var logMessage =
                    String.Format("Cannot create a FloatingHealthIndicator with DxVector2 {0}",
                        floatDistance);
                LOG.Error(logMessage);
                GenericUtils.HardFail(logMessage);
            }
        }

        public override void LoadContent()
        {
            foregroundTexture_ = new Texture2D(DxGame.GraphicsDevice, 1, 1);
            foregroundTexture_.SetData(new[] {foregroundColor_});
            backgroundTexture_ = new Texture2D(DxGame.GraphicsDevice, 1, 1);
            backgroundTexture_.SetData(new[] {backgroundColor_});
            base.LoadContent();
        }

        protected static Vector2 DetermineHealthBarOrigin(DxVector2 position,
            DxVector2 floatDistance) { return (position + floatDistance).ToVector2(); }

        public override void Draw(DxGameTime gameTime)
        {
            var origin = DetermineHealthBarOrigin(position_.Position, floatDistance_);
            var foregroundWidth =
                (int) Math.Ceiling(PercentHealthRemaining * HEALTH_BAR_PIXEL_WIDTH);
            spriteBatch_.Draw(foregroundTexture_,
                new Rectangle((int) origin.X, (int) origin.Y, foregroundWidth,
                    HEALTH_BAR_PIXEL_HEIGHT), foregroundColor_);
            spriteBatch_.Draw(backgroundTexture_,
                new Rectangle((int) origin.X + foregroundWidth, (int) origin.Y,
                    (HEALTH_BAR_PIXEL_WIDTH - foregroundWidth),
                    HEALTH_BAR_PIXEL_HEIGHT), backgroundColor_);
        }
    }
}