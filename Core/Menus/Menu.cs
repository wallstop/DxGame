using System.Collections.Generic;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public abstract class Menu : DrawableGameComponent
    {
        protected List<MenuItem> MenuItems { get; set; }
        protected SpriteFont menuItem_;

        public Menu(DxGame game) : base(game)
        {
        }
    }
}