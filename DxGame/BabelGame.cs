using System;
using BabelUILibrary.Core.Services;
using DxCore.Core.Services;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework;

namespace DxGame
{
    public sealed class BabelGame : DxCore.DxGame
    {
        private int OriginalScreenHeight { get; set; }
        private int OriginalScreenWidth { get; set; }

        public BabelGame()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            /* One time setup here */
            Graphics.DeviceCreated += HandleDeviceCreated;
            Graphics.PreparingDeviceSettings += HandlePreparingDeviceSettings;
        }

        private void HandleDeviceCreated(object sender, EventArgs eventArgs)
        {
            new MonoGameEngine(GraphicsDevice, OriginalScreenWidth, OriginalScreenHeight);
        }

        private void HandlePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            OriginalScreenWidth = Graphics.PreferredBackBufferWidth;
            OriginalScreenHeight = Graphics.PreferredBackBufferHeight;
        }

        protected override void Initialize()
        { 
            base.Initialize();
            UiService uiService = new UiService(new Root());
            uiService.Create();

            NetworkService netService = new NetworkService();
            netService.Create();

            new DeveloperService().Create();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            FontManager.Instance.LoadFonts(Content);
        }
    }
}
