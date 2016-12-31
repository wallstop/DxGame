using AnimationEditorLibrary.Core.Services.Components;
using DxCore.Core.Services;
using EmptyKeys.UserInterface.Generated;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Services
{
    public sealed class RootUiService : DxService
    {
        public Root UI { get; }

        private UiDrawer UiDrawer { get; set; }

        public RootUiService(Root rootUi)
        {
            Validate.Hard.IsNotNull(rootUi);
            UI = rootUi;
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(UiDrawer))
            {
                UiDrawer = new UiDrawer(UI);
                Self.AttachComponent(UiDrawer);
            }
        }
    }
}