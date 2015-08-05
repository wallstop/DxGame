using System;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DXGame.Core.GraphicsWidgets
{
    public class BlinkingCursor : DrawableComponent
    {
        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public Vector2 Origin { get; set; }
        public float Width { get; protected set; }
        public float Height { get; protected set; }
        public TimeSpan BlinkRate { get; protected set; }
        public bool Drawn { get; protected set; }
        public Color Color { get; protected set; }
        protected TimeSpan LastToggled { get; set; }
        protected Texture2D Texture { get; set; }

        public BlinkingCursor(DxGame game)
            : base(game)
        {
            // TODO: Move this kind of thing to Initialize()? IDK what the idiomatic approach is
            Drawn = false;
            LastToggled = TimeSpan.FromSeconds(0);
            WithBlinkRate(2);
        }

        public BlinkingCursor WithBlinkRate(float blinkRate)
        {
            return WithBlinkRate(TimeSpan.FromSeconds(1.0 / blinkRate));
        }

        public BlinkingCursor WithBlinkRate(TimeSpan blinkRate)
        {
            Validate.IsTrue(blinkRate > TimeSpan.FromSeconds(0),
                $"{GetType()} cannot be initialized with a negative TimeSpan ({blinkRate})");
            BlinkRate = blinkRate;
            return this;
        }

        public BlinkingCursor WithWidth(float width)
        {
            Validate.IsTrue(width > 0, $"{GetType()} cannot be initialized with a negative / 0 width ({width})");
            Width = width;
            return this;
        }

        public BlinkingCursor WithHeight(float height)
        {
            Validate.IsTrue(height > 0, $"{GetType()} cannot be initialized with a negative / 0 height ({height}");
            Height = height;
            return this;
        }

        public BlinkingCursor WithOrigin(Vector2 origin)
        {
            Origin = origin;
            return this;
        }

        public BlinkingCursor WithColor(Color color)
        {
            Color = color;
            return this;
        }

        protected override void Update(DxGameTime gameTime)
        {
            var totalTime = gameTime.TotalGameTime;
            if (totalTime > LastToggled + BlinkRate)
            {
                // Swap the "Drawn" state if we've exceeded our blink capacity
                Drawn = !Drawn;
                LastToggled = totalTime;
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // Only draw ourselves if we're in our "Drawn" state
            if (Check.IsNullOrDefault(Texture))
            {
                Texture = new Texture2D(DxGame.GraphicsDevice, (int) Width, (int) Height);
                Texture.SetData(Enumerable.Repeat(Color, (int) (Width * Height)).ToArray());
            }

            if (Drawn)
            {
                spriteBatch.Draw(Texture, Origin);
            }
        }
    }
}