using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DXGame.Core.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MainMenu : Menu
    {
        private Game game_;

        public MainMenu(Game game, GameModel gameState) : base(game, gameState)
        {
            Debug.Assert(game != null, "Main Menu cannot be initialized with a null game!");
            menuItem_ = Game.Content.Load<SpriteFont>("MainMenu");
            game_ = game;
        }

        public override void Initialize()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            MenuItem play = new MenuItem().WithText("Play").WithAction(PlayAction);
        }

        private void PlayAction()
        {
            Game.Components.Remove(this);
            Game.Components.Add(new GameModel(game_));
        }
    }
}
