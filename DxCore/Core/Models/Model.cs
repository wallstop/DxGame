using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
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

        public override void Create()
        {
            GameObject model = GameObject.From(this);
            model.Create();
            DxGame.Instance.AttachModel(this);
        }
    }
}