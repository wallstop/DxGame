using System;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        TODO: Wire this into a player's Buffs. This should be one-per player, but a standard component 
    */

    [Serializable]
    [DataContract]
    public class BuffBar : HudComponent
    {
        public BuffBar(DxGame game)
            : base(game)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}