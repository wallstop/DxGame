using DXGame.Core.GraphicsWidgets.HUD;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public class HudModel : Model
    {
        public HudRegion HudRegion { get; private set; }

        public HudModel()
        {
            // If we have this here, do we actually need a HudComponent at all?
            DrawPriority = DrawPriority.HUD_LAYER;
        }

        public override void Initialize()
        {
            HudRegion = new HudRegion();
        }
    }
}