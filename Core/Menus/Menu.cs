using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DXGame.Core.Menus
{
    public abstract class Menu : DrawableGameComponent
    {

        protected GameState GameState { get; set; }
        protected IEnumerable<MenuItem> MenuItems { get; set; }

        public Menu(Game game, GameState gameState) : base(game)
        {
            GameState = gameState;
        }

        protected abstract void InitializeMenu();

    }
}