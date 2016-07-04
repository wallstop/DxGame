using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace Babel.Menus
{
    public class MenuItem : Component, ISpatial
    {
        public delegate void MenuAction();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public string Text { get; set; }
        public MenuAction Action { get; set; }

        public DxVector2 WorldCoordinates => Space.TopLeft;

        public DxRectangle Space { get; set; }
        public SpriteFont SpriteFont { get; set; }

        public MenuItem WithText(string text)
        {
            Validate.Hard.IsNotNull(text, () => this.GetFormattedNullOrDefaultMessage(nameof(text)));
            Text = text;
            return this;
        }

        public MenuItem WithAction(MenuAction action)
        {
            Validate.Hard.IsNotNull(action, () => this.GetFormattedNullOrDefaultMessage(nameof(action)));
            Action = action;
            return this;
        }

        public MenuItem WithSpace(DxRectangle space)
        {
            Validate.Hard.IsNotNull(space, () => this.GetFormattedNullOrDefaultMessage(nameof(space)));
            Space = space;
            return this;
        }

        public MenuItem WithSpriteFont(SpriteFont spriteFont)
        {
            Validate.Hard.IsNotNull(spriteFont, () => this.GetFormattedNullOrDefaultMessage(nameof(spriteFont)));
            SpriteFont = spriteFont;
            return this;
        }

        public void OnAction()
        {
            if(!ReferenceEquals(Action, null))
            {
                Action();
            }
            else
            {
                Logger.Warn($"{nameof(OnAction)} called on {GetType()} {Text} but no action assigned.");
            }
        }

    }
}