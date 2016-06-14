using System;
using Babel.Menus;
using DxCore;
using DxCore.Core.Models;

namespace Babel.Main
{
    public class BabelGame : DxGame
    {
        protected override void SetUp()
        {
            Console.WriteLine(":^)");
            // TODO
        }

        protected override void Initialize()
        {
            MainMenu playMenu = new MainMenu();
            AddAndInitializeComponent(playMenu);

            FrameModel frameModel = new FrameModel();
            AttachModel(frameModel);

            NetworkModel netModel = new NetworkModel();
            AttachModel(netModel);

            InputModel inputModel = new InputModel();
            AttachModel(inputModel);

            CameraModel cameraModel = new CameraModel();
            AttachModel(cameraModel);
            base.Initialize();
        }
    }
}
