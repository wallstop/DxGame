using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class PlayMenu : Menu
    {
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

            MenuItems.Add(singlePlayer);
            MenuItems.Add(hostMultiplayer);
            MenuItems.Add(joinMultiplayer);
            base.Initialize();
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