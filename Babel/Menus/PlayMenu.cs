﻿using Babel.Models;
using DxCore;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace Babel.Menus
{
    public class PlayMenu : Menu
    {
        public override void Initialize()
        {
            var spriteFont = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Gungsuh_20");
            MenuItem singlePlayer =
                new MenuItem().WithText("Single Player")
                    .WithAction(SinglePlayerAction)
                    .WithSpriteFont(spriteFont)
                    .WithSpace(new DxRectangle(400, 400, 100, 100));
            // TODO: Base these off some centroid of screen

            MenuItem hostMultiplayer =
                new MenuItem().WithText("Host Multiplayer")
                    .WithAction(HostMultiplayer)
                    .WithSpriteFont(spriteFont)
                    .WithSpace(new DxRectangle(400, 500, 100, 100));

            MenuItem joinMultiplayer =
                new MenuItem().WithText("Join Multiplayer")
                    .WithAction(JoinMultiplayer)
                    .WithSpriteFont(spriteFont)
                    .WithSpace(new DxRectangle(400, 600, 100, 100));

            MenuItems.Add(singlePlayer);
            singlePlayer.Create();

            MenuItems.Add(hostMultiplayer);
            hostMultiplayer.Create();
            MenuItems.Add(joinMultiplayer);
            joinMultiplayer.Create();

            base.Initialize();
        }

        private void SinglePlayerAction()
        {
            new GameModel().Create();
            Remove();
        }

        private void HostMultiplayer()
        {
            EntityCreatedMessage hostMultiplayerMenuCreated = new EntityCreatedMessage(new HostMultiplayerMenu());
            hostMultiplayerMenuCreated.Emit();
            Remove();
        }

        private void JoinMultiplayer()
        {
            EntityCreatedMessage joinMultiplayerMenuCreated = new EntityCreatedMessage(new JoinMultiplayerMenu());
            joinMultiplayerMenuCreated.Emit();
            Remove();
        }
    }
}