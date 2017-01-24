using System;
using System.Collections.Generic;
using DxCore.Core.Components.Basic;
using DxCore.Core.Messaging;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Extension;
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
        private const int DefaultBorderThickness = 1;

        private static readonly Color DefaultColor = Color.Red;

        private SpriteBatchExtensions.BorderRenderMode BorderRenderMode { get; }

        private int BorderThickness { get; set; }

        private Func<List<DxRectangle>> BoundsProducer { get; }

        private Color Color { get; }

        private float Scale { get; set; } = 1.0f;

        public BoundsWidget(Func<List<DxRectangle>> boundsProducer, int? borderThickness = null, Color? color = null,
            SpriteBatchExtensions.BorderRenderMode borderRenderMode = SpriteBatchExtensions.DefaultBorderRenderMode)
        {
            Validate.Hard.IsNotNull(boundsProducer, () => this.GetFormattedNullOrDefaultMessage(nameof(boundsProducer)));
            BoundsProducer = boundsProducer;
            Color = color ?? DefaultColor;
            BorderThickness = borderThickness ?? DefaultBorderThickness;

            BorderRenderMode = borderRenderMode;

            DrawPriority = DrawPriority.Low;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            foreach(DxRectangle border in BoundsProducer.Invoke())
            {
                int compensatedThickness = (int) Math.Round(BorderThickness * Scale);
                compensatedThickness = MathHelper.Clamp(compensatedThickness, 1, 100);
                spriteBatch.DrawBorder(border, compensatedThickness, Color, BorderRenderMode);
            }
        }

        public override void OnAttach()
        {
            RegisterMessageHandler<ChangeScaleRequest>(HandleChangeScaleRequest);
        }

        private void HandleChangeScaleRequest(ChangeScaleRequest changeScale)
        {
            Scale = changeScale.Scale;
        }
    }
}