using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MenuItem
    {

        public delegate void MenuAction();

        public string Text { get; set; }
        public MenuAction Action { get; set; }

        public MenuItem()
        {
        }

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

        public void OnAction()
        {
            if(Action != null) 
                Action();
        }
    }
}
