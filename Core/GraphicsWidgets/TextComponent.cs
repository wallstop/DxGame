using System;
using System.Runtime.Serialization;
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
        public float AlphaBlend { get; set; } = 1.0f;

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