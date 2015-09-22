using DXGame.Core.Primitives;
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
            // TODO: Remove dependence on hardcoded font values
            var spriteFont = DxGame.Content.Load<SpriteFont>("Fonts/Gungsuh");
            MenuItem play =
                new MenuItem().WithText("Play")
                    .WithAction(PlayAction)
                    .WithSpriteFont(spriteFont)
                    .WithSpace(new DxRectangle(400, 400, 100, 100));
            MenuItems.Add(play);

            base.Initialize();
        }

        private void PlayAction()
        {
            DxGame.AddAndInitializeComponent(new PlayMenu(DxGame));
            Remove();
        }
    }
}