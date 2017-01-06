using AnimationEditorLibrary.Controls;
using AnimationEditorLibrary.Core.Services.Components;
using DxCore.Core.Services;
using EmptyKeys.UserInterface.Generated;
using WallNetCore.Validate;

namespace AnimationEditorLibrary.Core.Services
{
    public sealed class RootUiService : DxService
    {
        public Root UI { get; }

        public AnimationView View { get; }

        private UiDrawer UiDrawer { get; set; }

        public RootUiService(Root rootUi)
        {
            Validate.Hard.IsNotNull(rootUi);
            UI = rootUi;
            View = new AnimationView();
            UI.DataContext = View;

            // TODO: Register event handlers of sub-components
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