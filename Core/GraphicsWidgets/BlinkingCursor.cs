using System;
using System.Diagnostics;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Main;
using log4net;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets
{
    public class BlinkingCursor : DrawableComponent
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (BlinkingCursor));

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
            Debug.Assert(blinkRate > TimeSpan.FromSeconds(0),
                "Blinking Cursor cannot be initialized with a zero timespan!");
            BlinkRate = blinkRate;
            return this;
        }

        public BlinkingCursor WithWidth(float width)
        {
            Debug.Assert(width > 0, "Blinking Cursor cannot be initialized with a non-positive width");
            Width = width;
            return this;
        }

        public BlinkingCursor WithHeight(float height)
        {
            Debug.Assert(height > 0, "Blinking Cursor cannot be initialized with a non-positive height");
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

        public override void Update(GameTime gameTime)
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

        public override void Draw(GameTime gameTime)
        {
            // Only draw ourselves if we're in our "Drawn" state
            if (GenericUtils.IsNullOrDefault(Texture))
            {
                Texture = new Texture2D(GraphicsDevice, (int) Width, (int) Height);
                Texture.SetData(Enumerable.Repeat(Color, (int) (Width * Height)).ToArray());
            }

            if (Drawn)
            {
                spriteBatch_.Draw(Texture, Origin);
            }
            base.Draw(gameTime);
        }
    }
}