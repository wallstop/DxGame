using System.Diagnostics;

namespace DXGame.Core.Menus
{
    public class MenuItem
    {
        public delegate void MenuAction();

        public string Text { get; set; }
        public MenuAction Action { get; set; }

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
            if (Action != null)
                Action();
        }
    }
}