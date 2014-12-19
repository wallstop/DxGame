using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MainMenu : Menu
    {
        public MainMenu(Game game, GameState gameState) : base(game, gameState)
        {
            Debug.Assert(game != null, "Main Menu cannot be initialized with a null game!");
        }

        protected override void InitializeMenu()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            //MenuItem start = new MenuItem().WithSpriteFont(new SpriteFont())
            //throw new NotImplementedException();

        }
    }
}
