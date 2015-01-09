using System.Collections.Generic;
using System.Diagnostics;
using DXGame.Core.Models;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MainMenu : Menu
    {
        private readonly DxGame game_;

        public MainMenu(DxGame game) : base(game)
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