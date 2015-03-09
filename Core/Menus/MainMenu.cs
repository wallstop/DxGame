﻿using DXGame.Core.Models;
using DXGame.Core.Utils;
using DXGame.Main;
using Lidgren.Network;
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
                    .WithSpace(new Rectangle2f(400, 400, 100, 100));
            MenuItems.Add(play);

            var inputModel = new InputModel(DxGame);
            inputModel.Initialize();
            DxGame.AddAndInitializeComponent(inputModel);
            DxGame.AttachModel(inputModel);

            base.Initialize();
        }

        public override void Write(NetOutgoingMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override void Read(NetIncomingMessage message)
        {
            throw new System.NotImplementedException();
        }

        private void PlayAction()
        {
            Remove();
            DxGame.AddAndInitializeComponent(new PlayMenu(DxGame));
        }
    }
}