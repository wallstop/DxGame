using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Main;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Models
{
    [Serializable]
    [DataContract]
    public abstract class Model : DrawableComponent
    {
        protected Model(DxGame game)
            : base(game)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            // No-op in base
        }
    }
}