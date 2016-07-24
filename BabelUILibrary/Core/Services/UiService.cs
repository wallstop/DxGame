﻿using BabelUILibrary.Controls;
using DxCore;
using DxCore.Core;
using DxCore.Core.Primitives;
using DxCore.Core.Services;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework.Graphics;

namespace BabelUILibrary.Core.Models
{
    public class UiService : Service
    {
        public Root UI { get; }

        private RootController RootController { get; }

        public UiService(Root rootUi)
        {
            Validate.Hard.IsNotNullOrDefault(rootUi);
            DrawPriority = DrawPriority.MenuLayer;
            UI = rootUi;
            RootController = new RootController();
            rootUi.DataContext = RootController;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            UI.Draw(EmptyKeysGameTime(gameTime));
        }

        public static double EmptyKeysGameTime(DxGameTime gameTime)
        {
            return gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void LoadContent()
        {
            SpriteFont font = DxGame.Instance.Content.Load<SpriteFont>("Fonts/visitor_tt1_brk_26_regular");
            FontManager.DefaultFont = Engine.Instance.Renderer.CreateFont(font);
            FontManager.Instance.LoadFonts(DxGame.Instance.Content);
            ImageManager.Instance.LoadImages(DxGame.Instance.Content);
        }

        protected override void Update(DxGameTime gameTime)
        {
            UI.UpdateInput(EmptyKeysGameTime(gameTime));
            UI.UpdateLayout(EmptyKeysGameTime(gameTime));
            base.Update(gameTime);
        }
    }
}