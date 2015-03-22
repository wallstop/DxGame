using System;
using System.Diagnostics;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;
using log4net;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MenuItem
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MenuItem));

        public delegate void MenuAction();

        public string Text { get; set; }
        public MenuAction Action { get; set; }
        public DxRectangle Space { get; set; }
        public SpriteFont SpriteFont { get; set; }

        public MenuItem WithText(string text)
        {
            Debug.Assert(text != null, "Menu Item cannot be initialized with a null text");
            Text = text;
            return this;
        }

        public MenuItem WithAction(MenuAction action)
        {
            Debug.Assert(action != null, "Menu Item cannot be initialized with a null action");
            Action = action;
            return this;
        }

        public MenuItem WithSpace(DxRectangle space)
        {
            Debug.Assert(space != null, "Menu Item cannot be initialized with a null space");
            Space = space;
            return this;
        }

        public MenuItem WithSpriteFont(SpriteFont spriteFont)
        {
            Debug.Assert(spriteFont != null, "Menu Item cannot be initialized with a null SpriteFont");
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
                LOG.Warn(String.Format("Action called on MenuItem {0} but no action assigned.", Text));
            }
        }
    }
}