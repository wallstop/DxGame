using System;
using DxCore;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using MapEditorLibrary.Core;
using MapEditorLibrary.Core.Components;
using MapEditorLibrary.Core.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MapEditor : DxGame
    {
        private int NativeScreenWidth { get; set; }
        private int NativeScreenHeight { get; set; }

        public MapEditor()
        {
            Graphics.DeviceCreated += HandleDeviceCreated;
            Graphics.PreparingDeviceSettings += HandlePreparingDeviceSettings;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void HandleDeviceCreated(object sender, EventArgs eventArgs)
        {
            // What does this even do?
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

        protected override void SetUp() {}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: Offload
            RootUiModel uiRoot = new RootUiModel(new Root());
            uiRoot.Create();

            MapGridComponent mapGrid = new MapGridComponent(new DxUnit(3), 15, 15, 100, 100);
            mapGrid.Create();

            MousePanComponent mousePanner = new MousePanComponent();
            mousePanner.Create();

            DeveloperModel devModel = new DeveloperModel();
            DxGame.Instance.AttachModel(devModel);

            FontManager.Instance.LoadFonts(Content);
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
    }
}
