using DXGame.Core.Components.Basic;
using DXGame.Main;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        Simple base class for all HUD components that properly deals with assigning draw priority
    */

    public abstract class HudComponent : DrawableComponent
    {
        protected HudComponent(DxGame game)
            : base(game)
        {
            DrawPriority = DrawPriority.HUD_LAYER;
        }
    }
}