﻿using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DXGame.Core.Messaging;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Menus
{
    public class MainMenu : Menu
    {
        public override void Initialize()
        {
            // TODO: Remove dependence on hardcoded font values
            var spriteFont = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Gungsuh");
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
            EntityCreatedMessage playMenuCreated = new EntityCreatedMessage(new PlayMenu());
            playMenuCreated.Emit();
            Remove();
        }
    }
}