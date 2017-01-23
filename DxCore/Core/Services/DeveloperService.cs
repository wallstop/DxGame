using DxCore.Core.Components.Developer;
using DxCore.Core.GraphicsWidgets.HUD;
using DxCore.Core.Messaging;
using DxCore.Core.Messaging.Entity;
using DxCore.Core.Services.Components;
using DxCore.Core.Utils;
using WallNetCore.Validate;

namespace DxCore.Core.Services
{
    /**
        For debug purposes only :^)

        <summary> Simple debug-type Model used to display useful information in-game. Should not be used in production. </summary>
    */

    public class DeveloperService : DxService
    {
        private readonly GameElementCollection components_ = new GameElementCollection();
        private readonly DeveloperSwitch devSwitch_;

        public DeveloperMode DeveloperMode => devSwitch_.DeveloperMode;

        private DeveloperComponentHandler DevComponentHandler { get; set; }

        public DeveloperService()
        {
            var fpsTracker = new FpsWidget();
            components_.Add(fpsTracker);
            devSwitch_ = new DeveloperSwitch();
            WorldDrawer worldDrawer = new WorldDrawer();
            worldDrawer.Create();
            var teamCounterWidget = new TeamCounterWidget();
            components_.Add(teamCounterWidget);
            var timePerFrameBackground = new TimePerFrameGraphBackground();
            components_.Add(timePerFrameBackground);
            TimePerFrameGraph timePerFrameGraph = new TimePerFrameGraph();
            /* We need to let DxGame handle the TimePerFrameGraph - it draws at a different draw priority than we do */
            EntityCreatedMessage timePerFrameGraphCreated = new EntityCreatedMessage(timePerFrameGraph);
            timePerFrameGraphCreated.Emit();
            var keysPressed = new KeysPressedWidget();
            components_.Add(keysPressed);
            HealthAdjustor healthAdjustor = new HealthAdjustor();
            components_.Add(healthAdjustor);
        }

        protected override void OnCreate()
        {
            if(Validate.Check.IsNull(DevComponentHandler))
            {
                DevComponentHandler = new DeveloperComponentHandler(components_, devSwitch_);
                Self.AttachComponent(DevComponentHandler);
            }
        }
    }
}