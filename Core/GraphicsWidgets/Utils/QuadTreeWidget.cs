using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.GraphicsWidgets.Utils
{
    public delegate QuadTree<T> QuadTreeProducer<T>();

    public class QuadTreeWidget<T> : DrawableComponent
    {
        private readonly QuadTreeProducer<T> producer_;

        public QuadTreeWidget(DxGame game, QuadTreeProducer<T> quadTreeProducer) : base(game)
        {
            Validate.IsNotNullOrDefault(quadTreeProducer,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(quadTreeProducer)));
            producer_ = quadTreeProducer;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            QuadTree<T> quadTree = producer_();
            List<DxRectangle> quadrants = quadTree.Quadrants;
            foreach (var quadrant in quadrants)
            {
                SpriteBatchUtils.DrawBorder(DxGame, quadrant, 1, Color.DarkSlateBlue);
            }
        }
    }
}