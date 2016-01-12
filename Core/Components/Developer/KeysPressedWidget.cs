using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Basic;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Developer
{
    public class KeysPressedWidget : DrawableComponent
    {
        private static readonly int PIXEL_X_OFFSET = 250;
        private static readonly int PIXEL_Y_OFFSET = 5;
        private List<KeyboardEvent> currentEvents_ = new List<KeyboardEvent>();

        protected SpriteFont spriteFont_;

        public override void LoadContent()
        {
            spriteFont_ = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Pericles");
        }

        protected override void Update(DxGameTime gameTime)
        {
            InputModel inputModel = DxGame.Instance.Model<InputModel>();
            currentEvents_ = inputModel.Events.ToList();
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            string keyText =
                currentEvents_.Select(keyboardEvent => keyboardEvent.Key)
                    .Select(key => key.ToString())
                    .Aggregate("", (i, j) => i + " " + j);

            var screenRegion = DxGame.Instance.ScreenRegion;
            var drawLocation = new Vector2(Math.Abs(screenRegion.X) + PIXEL_X_OFFSET,
                Math.Abs(screenRegion.Y) + PIXEL_Y_OFFSET);

            Vector2 drawSize = spriteFont_.MeasureString(keyText);
            Texture2D blackTexture = TextureFactory.TextureForColor(Color.Black);
            const float transparencyWeight = 0.8f;
            Color transparency = ColorFactory.Transparency(transparencyWeight);
            /* Draw a neato transparent box behind the text to make the text "pop" */
            spriteBatch.Draw(blackTexture, color: transparency,
                destinationRectangle:
                    new Rectangle((int) drawLocation.X, (int) drawLocation.Y, (int) drawSize.X, (int) drawSize.Y));

            spriteBatch.DrawString(spriteFont_, keyText, drawLocation, Color.DarkOrange);
        }
    }
}