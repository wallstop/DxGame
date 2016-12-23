using System;
using DxCore;
using DxCore.Core;
using DxCore.Core.Services;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using MapEditorLibrary.Core.Components;
using MapEditorLibrary.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor
{
    public class MapEditor : DxGame
    {
        private int NativeScreenHeight { get; set; }
        private int NativeScreenWidth { get; set; }

        public MapEditor()
        {
            Graphics.DeviceCreated += HandleDeviceCreated;
            Graphics.PreparingDeviceSettings += HandlePreparingDeviceSettings;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Clean up init

            base.Initialize();
            RootUiService rootUiService = new RootUiService(new Root());
            rootUiService.Create();

            MapGridComponent mapGrid = new MapGridComponent(50, 50, 100, 100);
            GameObject mapGridObject = GameObject.From(mapGrid);
            mapGridObject.Create();

            GameObject mousePanner = GameObject.From(new MousePanComponent());
            mousePanner.Create();

            MapCreatorComponent mapCreator = new MapCreatorComponent(mapGrid);
            mapCreator.Create();
            mapGridObject.AttachComponent(mapCreator);

            DeveloperService devService = new DeveloperService();
            devService.Create();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            FontManager.Instance.LoadFonts(Content);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void HandleDeviceCreated(object sender, EventArgs eventArgs)
        {
            new MonoGameEngine(GraphicsDevice, NativeScreenWidth, NativeScreenHeight);
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
    }
}