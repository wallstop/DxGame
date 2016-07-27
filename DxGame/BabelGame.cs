using System;
using BabelUILibrary.Core.Services;
using DxCore.Core.Services;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework;

namespace DxGame
{
    public class BabelGame : DxCore.DxGame
    {
        private int NativeScreenWidth { get; set; }
        private int NativeScreenHeight { get; set; }

        public BabelGame()
        {
            Graphics.DeviceCreated += HandleDeviceCreated;
            Graphics.PreparingDeviceSettings += HandlePreparingDeviceSettings;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void HandleDeviceCreated(object sender, EventArgs eventArgs)
        {
            Engine engine = new MonoGameEngine(GraphicsDevice, NativeScreenWidth, NativeScreenHeight);
        }

        private void HandlePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs eventArgs)
        {
            NativeScreenWidth = eventArgs.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth;
            NativeScreenHeight = eventArgs.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight;
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
