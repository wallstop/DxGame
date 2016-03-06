﻿using DXGame.Core.Messaging.Entity;
using DXGame.Core.Models;
using DXGame.Core.Primitives;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Menus
{
    public class PlayMenu : Menu
    {
        public override void Initialize()
        {
            var spriteFont = DxGame.Instance.Content.Load<SpriteFont>("Fonts/Gungsuh");
            MenuItem singlePlayer = new MenuItem().WithText("Single Player")
                .WithAction(SinglePlayerAction)
                .WithSpriteFont(spriteFont)
                .WithSpace(new DxRectangle(400, 400, 100, 100));
            // TODO: Base these off some centroid of screen

            MenuItem hostMultiplayer = new MenuItem().WithText("Host Multiplayer")
                .WithAction(HostMultiplayer)
                .WithSpriteFont(spriteFont)
                .WithSpace(new DxRectangle(400, 500, 100, 100));

            MenuItem joinMultiplayer = new MenuItem().WithText("Join Multiplayer")
                .WithAction(JoinMultiplayer)
                .WithSpriteFont(spriteFont)
                .WithSpace(new DxRectangle(400, 600, 100, 100));

            MenuItems.Add(singlePlayer);
            MenuItems.Add(hostMultiplayer);
            MenuItems.Add(joinMultiplayer);
            base.Initialize();
        }

        private void SinglePlayerAction()
        {
            DxGame.Instance.AttachModel(new GameModel());
            Remove();
        }

        private void HostMultiplayer()
        {
            EntityCreatedMessage hostMultiplayerMenuCreated = new EntityCreatedMessage(new HostMultiplayerMenu());
            DxGame.Instance.BroadcastTypedMessage<EntityCreatedMessage>(hostMultiplayerMenuCreated);
            Remove();
        }

        private void JoinMultiplayer()
        {
            EntityCreatedMessage joinMultiplayerMenuCreated = new EntityCreatedMessage(new JoinMultiplayerMenu());
            DxGame.Instance.BroadcastTypedMessage<EntityCreatedMessage>(joinMultiplayerMenuCreated);
            Remove();
        }
    }
}