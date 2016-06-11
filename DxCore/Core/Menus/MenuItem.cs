using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace DxCore.Core.Menus
{
    public class MenuItem
    {
        public delegate void MenuAction();

        private static readonly Logger LOG = LogManager.GetCurrentClassLogger();
        public string Text { get; set; }
        public MenuAction Action { get; set; }
        public DxRectangle Space { get; set; }
        public SpriteFont SpriteFont { get; set; }

        public MenuItem WithText(string text)
        {
            Validate.IsNotNull(text, $"{GetType()} cannot be initialized with null text");
            Text = text;
            return this;
        }

        public MenuItem WithAction(MenuAction action)
        {
            Validate.IsNotNull(action, $"{GetType()} cannot be initialized with a null action");
            Action = action;
            return this;
        }

        public MenuItem WithSpace(DxRectangle space)
        {
            Validate.IsNotNull(space, $"{GetType()} cannot be initialized with a null space");
            Space = space;
            return this;
        }

        public MenuItem WithSpriteFont(SpriteFont spriteFont)
        {
            Validate.IsNotNull(spriteFont, $"{GetType()} cannot be initialized with a null SpriteFont");
            SpriteFont = spriteFont;
            return this;
        }

        public void OnAction()
        {
            if (Action != null)
            {
                Action();
            }
            else
            {
                LOG.Warn($"{nameof(OnAction)} called on {GetType()} {Text} but no action assigned.");
            }
        }
    }
}