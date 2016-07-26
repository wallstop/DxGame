using BabelUILibrary.Controls;
using BabelUILibrary.Core.Services.Components;
using DxCore.Core.Services;
using DxCore.Core.Utils.Validate;
using EmptyKeys.UserInterface.Generated;

namespace BabelUILibrary.Core.Services
{
    public class UiService : DxService
    {
        public Root UI { get; }

        private RootController RootController { get; }

        private UiDrawer UiDrawer { get; set; }

        public UiService(Root rootUi)
        {
            Validate.Hard.IsNotNullOrDefault(rootUi);
            UI = rootUi;
            RootController = new RootController();
            rootUi.DataContext = RootController;
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