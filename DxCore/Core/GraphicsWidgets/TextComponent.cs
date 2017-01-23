using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.GraphicsWidgets
{
    [DataContract]
    [Serializable]
    public class TextComponent : DrawableComponent
    {
        [DataMember] private readonly IPositional position_;

        [DataMember] private readonly string spriteFontName_;

        [NonSerialized] [IgnoreDataMember] private SpriteFont spriteFont_;

        [DataMember]
        public float AlphaBlend { get; set; } = 1.0f;

        [DataMember]
        public DxColor DxColor { get; set; }

        [DataMember]
        public string Text { get; set; }

        public TextComponent(IPositional position, SpriteFont spriteFont, string spriteFontName)
        {
            Validate.Hard.IsNotNullOrDefault(position, () => this.GetFormattedNullOrDefaultMessage(position));
            position_ = position;
            Validate.Hard.IsNotNullOrDefault(spriteFont, () => this.GetFormattedNullOrDefaultMessage(spriteFont));
            spriteFont_ = spriteFont;
            Validate.Hard.IsNotNullOrDefault(spriteFontName,
                () => this.GetFormattedNullOrDefaultMessage(nameof(spriteFontName)));
            spriteFontName_ = spriteFontName;
            Text = "";
            DxColor = new DxColor(Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.DrawString(spriteFont_, Text, position_.WorldCoordinates.Vector2, DxColor.Color);
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