using System;
using DxCore;
using DxCore.Core;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Settings;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework.Graphics;
using NLog;

namespace BabelUILibrary.Core.Services.Components
{
    public sealed class UiDrawer : DrawableComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Root UI { get; }

        private Action GraphicsUpdatedDelegate { get; }

        private bool RefreshEngine { get; set; }

        public UiDrawer(Root ui)
        {
            DrawPriority = DrawPriority.MenuLayer;
            Validate.Hard.IsNotNull(ui);
            UI = ui;

            GraphicsUpdatedDelegate = RegisterMonogameEngine;
            DxGame.Instance.GameSettings.VideoSettings.RegisterPropertyChangeListener(GraphicsUpdatedDelegate);
            RefreshEngine = true;
        }

        private void RegisterMonogameEngine()
        {
            RefreshEngine = true;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            if(RefreshEngine)
            {
                RefreshEngine = false;
                Logger.Debug(DxGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode);
                int width;
                int height;
                int uiWidth = DxGame.Instance.GameSettings.VideoSettings.ScreenWidth;
                int uiHeight = DxGame.Instance.GameSettings.VideoSettings.ScreenHeight;
                if(DxGame.Instance.GameSettings.VideoSettings.WindowMode == WindowMode.Fullscreen ||
                   DxGame.Instance.GameSettings.VideoSettings.WindowMode == WindowMode.Borderless)
                {
                    width = DxGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
                    height = DxGame.Instance.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
                }
                else
                {
                    width = uiWidth;
                    height = uiHeight;
                }
                new MonoGameEngine(DxGame.Instance.GraphicsDevice, width, height);
                UI.Resize(uiWidth, uiHeight);
            }
            UI.Draw(EmptyKeysGameTime(gameTime));
        }

        public static double EmptyKeysGameTime(DxGameTime gameTime)
        {
            return gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public override void LoadContent()
        {
            SpriteFont font = DxGame.Instance.Content.Load<SpriteFont>("Fonts/visitor_tt1_brk_15_Regular");
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
