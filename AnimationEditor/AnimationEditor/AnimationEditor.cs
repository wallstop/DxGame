using System;
using AnimationEditorLibrary.Core.Services;
using DxCore;
using DxCore.Core.Services;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Generated;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimationEditor
{
    public sealed class AnimationEditor : DxGame
    {
        private int NativeScreenHeight { get; set; }
        private int NativeScreenWidth { get; set; }

        public AnimationEditor()
        {
            Graphics.DeviceCreated += HandleDeviceCreated;
            Graphics.PreparingDeviceSettings += HandlePreparingDeviceSettings;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            RootUiService rootUi = new RootUiService(new Root());
            rootUi.Create();

            DeveloperService devService = new DeveloperService();
            devService.Create();

            AnimationViewerHudService hud = new AnimationViewerHudService(() => rootUi.View.CurrentFrameView,
                () => rootUi.View.OtherFrameView);
            hud.Create();
            // TODO: Get rid of cyclic dependency :(
            rootUi.View.Offset = () => hud.SpriteSheetOffset;
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