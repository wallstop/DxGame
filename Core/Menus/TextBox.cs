using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DXGame.Core.Components.Advanced;
using DXGame.Core.Components.Basic;
using DXGame.Core.Input;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace DXGame.Core.Menus
{
    public class TextBox : DrawableComponent
    {
        public string Text { get; private set; }
        private int CursorPosition { get; set; }
        private SpriteFont SpriteFont { get; set; }
        public SpatialComponent SpatialComponent { get; private set; }
        private Texture2D Texture { get; set; }
        private Color TextColor { get; set; }

        public TextBox(DxGame game)
            : base(game)
        {
            Text = "";
            CursorPosition = 0;
        }

        public TextBox WithSpriteFont(SpriteFont spriteFont)
        {
            GenericUtils.CheckNullOrDefault(spriteFont, "Cannot instantiate a TextBox with a null SpriteFont!");
            SpriteFont = spriteFont;
            return this;
        }

        public TextBox WithSpatialComponent(SpatialComponent spatialComponent)
        {
            GenericUtils.CheckNullOrDefault(spatialComponent,
                "Cannot instantiate a TextBox with a null Spatial Component!");
            SpatialComponent = spatialComponent;
            var width = (int) SpatialComponent.Width;
            var height = (int) SpatialComponent.Height;
            Texture = new Texture2D(GraphicsDevice, width, height);
            return WithBackGroundColor(Color.White);
        }

        public TextBox WithBackGroundColor(Color color)
        {
            GenericUtils.CheckNullOrDefault(Texture,
                String.Format("Cannot initialize a TextBox with color {0} without a valid Texture", color));
            var width = (int) SpatialComponent.Width;
            var height = (int) SpatialComponent.Height;
            // Enumerable.Repeat is very slow - might want to use for loop
            Texture.SetData(Enumerable.Repeat(color, width * height).ToArray());
            return this;
        }

        public TextBox WithTextColor(Color color)
        {
            TextColor = color;
            return this;
        }

        public override void Draw(GameTime gameTime)
        {
            GenericUtils.CheckNullOrDefault(SpatialComponent);
            GenericUtils.CheckNullOrDefault(SpriteFont);

            spriteBatch_.Draw(Texture, SpatialComponent.Position);
            spriteBatch_.DrawString(SpriteFont, Text, SpatialComponent.Position, TextColor);
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var mousePosition = mouseState.Position;

            // Only update if we have focus
            if (SpatialComponent.Space.Contains(mousePosition))
            {
                var inputModel = DxGame.Model<InputModel>();
                IEnumerable<KeyboardEvent> finishedKeys = inputModel.FinishedEvents;
                HandleKeyboardEvents(finishedKeys);
                IEnumerable<KeyboardEvent> longPressedKeys =
                    inputModel.Events.Where(key => (key.HeldDown && !KeyboardEvent.KeyCharacters.ContainsKey(key.Key)));
                HandleKeyboardEvents(longPressedKeys);
            }
            base.Update(gameTime);
        }

        /*
            For all keys in the given input, checks to see if:
            -Valid character 
                OR
            -Left/Right Key

            And either updates the position of the cursor in the text box (not currently drawn)
                OR
            Types the text from the keys. Note: If multiple keys are down / long pressed,
            you will get shit tons of characters.
        */

        private void HandleKeyboardEvents(IEnumerable<KeyboardEvent> events)
        {
            string preCursor = Text.Substring(0, CursorPosition);
            string postCursor = Text.Substring(CursorPosition);
            string typedText = "";
            foreach (KeyboardEvent keyEvent in events)
            {
                var minCursorPos = Math.Max(0, CursorPosition - 1);
                switch (keyEvent.Key)
                {
                case Keys.Left:
                    CursorPosition = minCursorPos;
                    break;
                case Keys.Right:
                    ++CursorPosition;
                    break;
                case Keys.Back: // Backspace - delete 1 char (to a max of the front of the word)
                    preCursor = preCursor.Substring(0, minCursorPos);
                    CursorPosition = minCursorPos;
                    break;
                case Keys.Delete: // Delete - delete 1 char to a max of the back of the word
                    postCursor = Text.Substring(Math.Min(1, postCursor.Length));
                    break;
                default:
                    if (KeyboardEvent.KeyCharacters.ContainsKey(keyEvent.Key))
                    {
                        // If this is slow, use a StringBuilder (don't care enough right now)
                        typedText += KeyboardEvent.KeyCharacters[keyEvent.Key];
                        ++CursorPosition;
                    }
                    break;
                }
            }

            Text = preCursor + typedText + postCursor;
        }
    }
}