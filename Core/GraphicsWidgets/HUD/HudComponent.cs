using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    public abstract class HudComponent : DrawableComponent
    {
        protected HudComponent(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.HUD_LAYER;
        }
    }
}