using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        TODO: Wire this into a player's abilities. This should end up involving a Factory that creates these bad boys
    */

    public class AbilityBar : HudComponent
    {
        public AbilityBar(DxGame game)
            : base(game)
        {
        }

        public override void Draw(DxGameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
    }
}