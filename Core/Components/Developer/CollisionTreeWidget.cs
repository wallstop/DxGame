using System.Collections.Generic;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using DXGame.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DXGame.Core.Components.Developer
{
    public delegate ICollisionTree<T> CollisionTreeProducer<T>();

    /**
        <summary>
            Draws all of the bounding boxes & spatial divisions of the CollisionTree that is produced with the specified function.
            Useful for debugging.        
        </summary>
    */

    public class CollisionTreeWidget<T> : DrawableComponent
    {
        private readonly CollisionTreeProducer<T> producer_;
        public override bool ShouldSerialize => false;

        public CollisionTreeWidget(DxGame game, CollisionTreeProducer<T> collisionTreeProducer)
            : base(game)
        {
            Validate.IsNotNullOrDefault(collisionTreeProducer,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(collisionTreeProducer)));
            producer_ = collisionTreeProducer;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            ICollisionTree<T> collisionTree = producer_();
            /* Draw each "collision rectangle" */
            List<DxRectangle> divisions = collisionTree.Divisions;
            foreach (var division in divisions)
            {
                spriteBatch.DrawBorder(division, 1, Color.DarkSlateBlue);
            }
            /* ...as well as each Node in the tree */
            List<DxRectangle> nodes = collisionTree.Nodes;
            foreach (var nodeBoundary in nodes)
            {
                spriteBatch.DrawBorder(nodeBoundary, 1, Color.Black);
            }
        }
    }
}