using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxCore.Core.Components.Basic;
using DxCore.Core.Models;
using DxCore.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Components.Developer
{
    public class WorldDrawer : DrawableComponent
    {
        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            WorldModel worldModel = DxGame.Instance.Model<WorldModel>();
            if(ReferenceEquals(worldModel, null))
            {
                
            }
        }
    }
}
