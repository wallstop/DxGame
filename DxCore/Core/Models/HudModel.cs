using DxCore.Core.GraphicsWidgets.HUD;

namespace DxCore.Core.Models
{
    public class HudModel : Model
    {
        public HudRegion HudRegion { get; private set; }

        public HudModel()
        {
            // If we have this here, do we actually need a HudComponent at all?
            DrawPriority = DrawPriority.HudLayer;
        }

        public override void Initialize()
        {
            HudRegion = new HudRegion();
        }
    }
}