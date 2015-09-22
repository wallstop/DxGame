using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.HUD
{
    [Serializable]
    [DataContract]
    public class HudRegion : HudComponent
    {

        public HudRegion(DxGame game)
            : base(game)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}