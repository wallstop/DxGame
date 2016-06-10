using System;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.GraphicsWidgets.HUD
{
    [Serializable]
    [DataContract]
    public class HudRegion : HudComponent
    {
        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}