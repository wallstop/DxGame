using System;
using System.Runtime.Serialization;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    /*
        TODO: Wire this into a player's abilities. This should end up involving a Factory that creates these bad boys
    */
    [Serializable]
    [DataContract]
    public class AbilityBar : HudComponent
    {
        public AbilityBar(DxGame game)
            : base(game)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new System.NotImplementedException();
        }
    }
}