using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Advanced.Position;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.GraphicsWidgets.HUD
{
    [DataContract]
    [Serializable]
    public sealed class TransparentBackground : DrawableComponent
    {
        [IgnoreDataMember] [NonSerialized] private Texture2D transparentBackground_;

        [DataMember]
        private DxColor Color { get; set; }

        private Rectangle DrawArea
        {
            get
            {
                DxVector2 worldCoordinates = Position.WorldCoordinates;
                Point dimensions = new Point(Width(), Height());
                return new Rectangle(worldCoordinates, dimensions);
            }
        }

        [DataMember]
        private Func<int> Height { get; set; }

        [DataMember]
        private IPositional Position { get; set; }

        [DataMember]
        private Func<int> Width { get; set; }

        private TransparentBackground(DrawPriority priority, IPositional position, DxColor color, Func<int> width,
            Func<int> height)
        {
            Position = position;
            Width = width;
            Height = height;
            Color = color;
            DrawPriority = priority;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            spriteBatch.Draw(transparentBackground_, DrawArea, Color.Color);
        }

        public override void Initialize()
        {
            transparentBackground_ = TextureFactory.TextureForColor(Color.Color);
            base.Initialize();
        }

        public class TransparentBackgroundBuilder : IBuilder<TransparentBackground>
        {
            private const float DefaultTransparency = 0.7f;
            private const DrawPriority DefaultDrawPriority = DrawPriority.Normal;
            private static readonly DxColor DefaultColor = new DxColor(Microsoft.Xna.Framework.Color.Gray);

            private DxColor Color { get; set; } = DefaultColor;
            private Func<int> Height { get; set; }
            private IPositional Position { get; set; }
            private DrawPriority Priority { get; set; } = DefaultDrawPriority;
            private float Transparency { get; set; } = DefaultTransparency;

            private DxColor TransparentColor
            {
                get
                {
                    // TODO: Make sure callers are totally aware we're making their color choice transparent
                    Color color = ColorFactory.Transparency(Transparency, Color.Color);
                    DxColor newColor = new DxColor(color);
                    return newColor;
                }
            }

            private Func<int> Width { get; set; }

            public TransparentBackground Build()
            {
                Validate.Hard.IsNotNull(Position);
                Validate.Hard.IsNotNull(Width);
                Validate.Hard.IsNotNull(Height);

                return new TransparentBackground(Priority, Position, TransparentColor, Width, Height);
            }

            public TransparentBackgroundBuilder WithColor(DxColor color)
            {
                Color = color;
                return this;
            }

            public TransparentBackgroundBuilder WithHeight(Func<int> height)
            {
                Height = height;
                return this;
            }

            public TransparentBackgroundBuilder WithPosition(IPositional position)
            {
                Position = position;
                return this;
            }

            public TransparentBackgroundBuilder WithPriority(DrawPriority priority)
            {
                Priority = priority;
                return this;
            }

            public TransparentBackgroundBuilder WithTransparency(float transparency)
            {
                Transparency = transparency;
                return this;
            }

            public TransparentBackgroundBuilder WithWidth(Func<int> width)
            {
                Width = width;
                return this;
            }
        }
    }
}