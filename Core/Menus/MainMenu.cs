using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class MainMenu : Menu
    {
        public MainMenu(DxGame game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/Gungsuh");
            MenuItem play =
                new MenuItem().WithText("Play")
                    .WithAction(PlayAction)
                    .WithSpriteFont(spriteFont)
                    .WithSpace(new Rectangle2f(400, 400, 100, 100));
            MenuItems.Add(play);
            base.Initialize();
        }

        private void PlayAction()
        {
            Remove();
            DxGame.AddAndInitializeComponent(new PlayMenu(DxGame));
        }
    }
}