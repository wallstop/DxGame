using System.Collections;
using System.Collections.Generic;
using DXGame.Core.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public abstract class Menu : DrawableGameComponent
    {

        protected GameModel GameState { get; set; }
        protected IEnumerable<MenuItem> MenuItems { get; set; }
        protected SpriteFont menuItem_;

        public Menu(Game game, GameModel gameState) : base(game)
        {
            GameState = gameState;
        }

    }
}