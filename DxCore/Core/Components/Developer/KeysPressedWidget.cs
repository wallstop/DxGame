using System.Collections.Generic;
using System.Linq;
using DxCore.Core.Components.Basic;
using DxCore.Core.Input;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Developer
{
    /**
        <summary>
            Takes all of the keys that are currently being pressed, converts them to a string, and draws them.
            This is very useful for video-capture demons, when it is hard to tell "what is going on"
        </summary>
    */

    public class KeysPressedWidget : DrawableComponent
    {
        private static readonly DxVector2 OFFSET = new DxVector2(250, 5);
        private List<KeyboardEvent> currentEvents_ = new List<KeyboardEvent>();

        protected SpriteFont spriteFont_;

        public override bool ShouldSerialize => false;

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

            Vector2 drawLocation = DxGame.Instance.OffsetFromScreen(OFFSET);

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