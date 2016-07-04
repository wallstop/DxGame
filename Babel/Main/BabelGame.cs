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
            playMenu.Create();

            FrameModel frameModel = new FrameModel();
            frameModel.Create();

            NetworkModel netModel = new NetworkModel();
            netModel.Create();


            new DeveloperModel().Create();

            base.Initialize();
        }
    }
}
