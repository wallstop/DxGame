using System;
using DXGame.Core.Wrappers;
using DXGame.Main;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        TODO: Wire this into a player's Buffs. This should be one-per player, but a standard component 
    */

    public class BuffBar : HudComponent
    {
        public BuffBar(DxGame game)
            : base(game)
        {
        }

        public override void Draw(DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}