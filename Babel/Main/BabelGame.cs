using System;
using Babel.Generators;
using Babel.Level;
using Babel.Menus;
using DxCore;
using DxCore.Core.Generators;
using DxCore.Core.Level;
using DxCore.Core.Models;
using DxCore.Core.Primitives;

namespace Babel.Main
{
    public class BabelGame : DxGame
    {
        private readonly BabelPlayerGenerator babelPlayerGenerator_ = new BabelPlayerGenerator(DxVector2.EmptyVector);
        private readonly SimpleRotatingLevelProgression levelProgression_ = new SimpleRotatingLevelProgression();

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
