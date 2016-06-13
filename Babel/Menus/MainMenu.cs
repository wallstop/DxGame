using DxCore;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace Babel.Menus
{
    public class MainMenu : Babel.Menus.Menu
    {
        public override void Initialize()
        {
            // TODO: Remove dependence on hardcoded font values
            var spriteFont = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Gungsuh");
            Babel.Menus.MenuItem play =
                new Babel.Menus.MenuItem().WithText("Play")
                    .WithAction(PlayAction)
                    .WithSpriteFont(spriteFont)
                    .WithSpace(new DxRectangle(400, 400, 100, 100));
            MenuItems.Add(play);

            base.Initialize();
        }

        private void PlayAction()
        {
            EntityCreatedMessage playMenuCreated = new EntityCreatedMessage(new Babel.Menus.PlayMenu());
            playMenuCreated.Emit();
            Remove();
        }
    }
}