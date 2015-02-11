using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXGame.Core.Components.Advanced;
using DXGame.Core.GraphicsWidgets;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class HostMultiplayerMenu : Menu
    {
        public HostMultiplayerMenu(DxGame game) 
            : base(game)
        {
        }

        public override void Initialize()
        {
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/ComicSans");

            var spatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new Vector2 {X = 200.0f, Y = spriteFont.LineSpacing}).WithPosition(600, 500);

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            var portBox =
                new TextBox(DxGame).WithSpatialComponent(spatial)
                    .WithBackGroundColor(Color.White)
                    .WithTextColor(Color.Black)
                    .WithMaxLength(5);

            

            base.Initialize();
        }
    }
}
