using DXGame.Core.Components.Advanced;
using DXGame.Core.GraphicsWidgets;
using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class PlayMenu : Menu
    {
        // TODO: Remove, this is mainly for testing
        private TextBox ScratchBox { get; set; }

        public PlayMenu(DxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/Gungsuh");
            MenuItem singlePlayer = new MenuItem().WithText("Single Player")
                .WithAction(SinglePlayerAction)
                .WithSpriteFont(spriteFont)
                .WithSpace(new Rectangle2f(400, 400, 100, 100));
            // TODO: Base these off some centroid of screen

            MenuItem hostMultiplayer = new MenuItem().WithText("Host Multiplayer")
                .WithAction(HostMultiplayer)
                .WithSpriteFont(spriteFont)
                .WithSpace(new Rectangle2f(400, 500, 100, 200));

            MenuItem joinMultiplayer = new MenuItem().WithText("Join Multiplayer")
                .WithAction(JoinMultiplayer)
                .WithSpriteFont(spriteFont)
                .WithSpace(new Rectangle2f(400, 600, 100, 200));

            SetupTextBox();

            MenuItems.Add(singlePlayer);
            MenuItems.Add(hostMultiplayer);
            MenuItems.Add(joinMultiplayer);
            base.Initialize();
        }

        // TODO: Remove, this is mainly for testing
        private void SetupTextBox()
        {
            var spatial = (SpatialComponent)
                new SpatialComponent(DxGame).WithDimensions(new Vector2 {X = 200.0f, Y = 50.0f}).WithPosition(600, 500);
            var textBox =
                new TextBox(DxGame).WithSpatialComponent(spatial)
                    .WithBackGroundColor(Color.WhiteSmoke)
                    .WithTextColor(Color.Black);

            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/Gungsuh");

            textBox.WithSpriteFont(spriteFont);

            ScratchBox = textBox;
            DxGame.AddAndInitializeComponent(ScratchBox);
        }

        public override void Remove()
        {
            DxGame.RemoveComponent(ScratchBox);
            base.Remove();
        }

        private void SinglePlayerAction()
        {
            Remove();
            DxGame.AddAndInitializeComponent(new GameModel(DxGame));
        }

        private void HostMultiplayer()
        {
            Remove();
            DxGame.AddAndInitializeComponent(new HostMultiplayerMenu(DxGame));
        }

        private void JoinMultiplayer()
        {
            Remove();
            DxGame.AddAndInitializeComponent(new JoinMultiplayerMenu(DxGame));
        }
    }
}