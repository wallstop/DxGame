using DXGame.Core.Components.Advanced;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Input;
using DXGame.Core.Utils;
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

            var portBoxSpatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new Vector2
                {
                    X = 200.0f,
                    Y = spriteFont.LineSpacing + 2 /* wiggle room for cursor */
                }).WithPosition(600, 500);

            // Ports have a range of 0 - 65536 (2 ^ 16 - 1) -> max length of 5
            var portBox =
                new TextBox(DxGame).WithSpatialComponent(portBoxSpatial)
                                   .WithBackGroundColor(Color.White)
                                   .WithTextColor(Color.Black)
                                   .WithMaxLength(5)
                    // Only allow numeric values for ports
                                   .WithValidKeys(KeyboardEvent.NumericKeys)
                                   .WithSpriteFont(spriteFont);

            var portLabel =
                new MenuItem().WithSpriteFont(spriteFont)
                              .WithText("Port:")
                              .WithSpace(new Rectangle2f
                              {
                                  X = portBoxSpatial.Space.X - /* Pixel Width of "Port:" */ spriteFont.MeasureString("Port:").X,
                                  Y = portBoxSpatial.Space.Y, 
                                  Width = portBoxSpatial.Width,
                                  Height = portBoxSpatial.Height
                              });

            MenuItems.Add(portLabel);
            DxGame.AddAndInitializeComponent(portBox);

            base.Initialize();
        }
    }
}