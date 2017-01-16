using System;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Developer
{
    /**
        <summary> 
            Simple developer tool that aims to draw red boxes around the target area 
        </summary>
        <note>
            This is very similar to BoundingBoxWidget, except it only draws a specific bounding box.
        </note>
        TODO: Consolidate with BoundingBoxWidget
    */

    public class BoundsWidget : DrawableComponent
    {
        private static readonly Color DefaultColor = Color.Red;

        private Func<DxRectangle> BoundsProducer { get; }

        private Color Color { get; }

        public BoundsWidget(Func<DxRectangle> boundsProducer, Color? color = null)
        {
            Validate.Hard.IsNotNull(boundsProducer, () => this.GetFormattedNullOrDefaultMessage(nameof(boundsProducer)));
            BoundsProducer = boundsProducer;
            Color = color ?? DefaultColor;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            DxRectangle border = BoundsProducer();
            spriteBatch.DrawBorder(border, 1, Color);
        }
    }
}