using AnimationEditorLibrary.Core.Services;
using DxCore;
using DxCore.Core.Services;
using EmptyKeys.UserInterface.Generated;

namespace AnimationEditor
{
    public sealed class AnimationEditor : DxGame
    {
        private int NativeScreenHeight { get; set; }
        private int NativeScreenWidth { get; set; }



        public AnimationEditor()
        {
            
        }

        protected override void Initialize()
        {
            base.Initialize();

            RootUiService rootUi = new RootUiService(new Root());
            rootUi.Create();



            DeveloperService devService = new DeveloperService();
            devService.Create();
        }


    }
}