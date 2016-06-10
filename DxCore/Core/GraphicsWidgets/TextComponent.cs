using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DxCore.Core.GraphicsWidgets
{
    [DataContract]
    [Serializable]
    public class TextComponent : DrawableComponent
    {
        [DataMember] private readonly PositionalComponent position_;

        [NonSerialized] [IgnoreDataMember] private SpriteFont spriteFont_;

        [DataMember] private readonly string spriteFontName_;

        [DataMember]
        public float AlphaBlend { get; set; } = 1.0f;

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public DxColor DxColor { get; set; }

        public TextComponent(PositionalComponent position, SpriteFont spriteFont, string spriteFontName)
        {
            Validate.IsNotNullOrDefault(position, this.GetFormattedNullOrDefaultMessage(position));
            position_ = position;
            Validate.IsNotNullOrDefault(spriteFont, this.GetFormattedNullOrDefaultMessage(spriteFont));
            spriteFont_ = spriteFont;
            Validate.IsNotNullOrDefault(spriteFontName, this.GetFormattedNullOrDefaultMessage(nameof(spriteFontName)));
            spriteFontName_ = spriteFontName;
            Text = "";
            DxColor = new DxColor(Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.DrawString(spriteFont_, Text, position_.Position.ToVector2(), DxColor.Color);
        }

        public override void LoadContent()
        {
            if(ReferenceEquals(spriteFont_, null))
            {
                spriteFont_ = DxGame.Instance.Content.Load<SpriteFont>(spriteFontName_);
            }
            base.LoadContent();
        }
    }
}