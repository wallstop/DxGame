using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public abstract class Menu : DrawableGameComponent
    {

        protected GameState GameState { get; set; }
        protected IEnumerable<MenuItem> MenuItems { get; set; }
        protected SpriteFont menuItem_;

        public Menu(Game game, GameState gameState) : base(game)
        {
            GameState = gameState;
        }

    }
}