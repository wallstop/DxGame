using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DXGame.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.Models
{
    [Serializable]
    [DataContract]
    public abstract class Model : DrawableComponent
    {
        protected Model()
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // No-op in base
        }
    }
}