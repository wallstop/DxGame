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
    public delegate ICollisionTree<T> CollisionTreeProducer<T>();

    public class CollisionTreeWidget<T> : DrawableComponent
    {
        private readonly CollisionTreeProducer<T> producer_;

        public CollisionTreeWidget(DxGame game, CollisionTreeProducer<T> quadTreeProducer) : base(game)
        {
            Validate.IsNotNullOrDefault(quadTreeProducer,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(quadTreeProducer)));
            producer_ = quadTreeProducer;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            ICollisionTree<T> quadTree = producer_();
            List<DxRectangle> divisions = quadTree.Divisions;
            foreach (var division in divisions)
            {
                SpriteBatchUtils.DrawBorder(DxGame, division, 1, Color.DarkSlateBlue);
            }
        }
    }
}