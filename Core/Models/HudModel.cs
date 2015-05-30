using DXGame.Core.Components.Basic;
using DXGame.Core.GraphicsWidgets.HUD;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.Models
{
    public class HudModel : Model
    {
        private HudRegion HudRegion;

        public HudModel(DxGame game)
            : base(game)
        {
            // If we have this here, do we actually need a HudComponent at all?
            DrawPriority = DrawPriority.HUD_LAYER;
        }

        public override void Initialize()
        {
            HudRegion = new HudRegion(DxGame);
        }

        public override void Draw(DxGameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}