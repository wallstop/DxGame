using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Core.Wrappers;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Developer
{
    public delegate ICollisionTree<T> CollisionTreeProducer<T>();

    public class CollisionTreeWidget<T> : DrawableComponent
    {
        private readonly CollisionTreeProducer<T> producer_;

        public CollisionTreeWidget(DxGame game, CollisionTreeProducer<T> collisionTreeProducer) : base(game)
        {
            Validate.IsNotNullOrDefault(collisionTreeProducer,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(collisionTreeProducer)));
            producer_ = collisionTreeProducer;
        }

        public override bool ShouldSerialize => false;

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            ICollisionTree<T> collisionTree = producer_();
            /* Draw each "collision rectangle" */
            List<DxRectangle> divisions = collisionTree.Divisions;
            foreach (var division in divisions)
            {
                SpriteBatchUtils.DrawBorder(DxGame, division, 1, Color.DarkSlateBlue);
            }
            /* ...as well as each Node in the tree */
            List<DxRectangle> nodes = collisionTree.Nodes;
            foreach (var nodeBoundary in nodes)
            {
                SpriteBatchUtils.DrawBorder(DxGame, nodeBoundary, 1, Color.Black);
            }
        }
    }
}