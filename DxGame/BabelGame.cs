using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babel.Menus;
using BabelUILibrary.Core.Models;
using DxCore.Core.Models;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            NativeScreenWidth = Graphics.PreferredBackBufferWidth;
            NativeScreenHeight = Graphics.PreferredBackBufferHeight;

            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.PreferMultiSampling = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            eventArgs.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 16;
        }

        protected override void Initialize()
        {
            base.Initialize();
            UiModel uiModel = new UiModel(new Root());
            uiModel.Create();

            MainMenu playMenu = new MainMenu();
            playMenu.Create();

            FrameModel frameModel = new FrameModel();
            frameModel.Create();

            NetworkModel netModel = new NetworkModel();
            netModel.Create();

            new DeveloperModel().Create();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            FontManager.Instance.LoadFonts(Content);
        }
    }
}
