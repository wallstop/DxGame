using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Components.Advanced.Position;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets
{
    [DataContract]
    [Serializable]
    public class TextComponent : DrawableComponent
    {
        [DataMember] private readonly PositionalComponent position_;

        [DataMember] private readonly SpriteFont spriteFont_;

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public Color Color { get; set; }

        public TextComponent(PositionalComponent position, SpriteFont spriteFont)
        {
            Validate.IsNotNullOrDefault(position, StringUtils.GetFormattedNullOrDefaultMessage(this, position));
            position_ = position;
            Validate.IsNotNullOrDefault(spriteFont, StringUtils.GetFormattedNullOrDefaultMessage(this, spriteFont));
            spriteFont_ = spriteFont;
            Text = "";
            Color = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.DrawString(spriteFont_, Text, position_.Position.ToVector2(), Color);
        }
    }
}
