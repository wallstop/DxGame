using System.Collections.Generic;
using DxCore.Core.Components.Basic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Distance;
using DxCore.Extension;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WallNetCore.Validate;

namespace DxCore.Core.Components.Developer
{
    public delegate ISpatialTree<T> CollisionTreeProducer<T>();

    /**
        <summary>
            Draws all of the bounding boxes & spatial divisions of the CollisionTree that is produced with the specified function.
            Useful for debugging.        
        </summary>
    */

    public class CollisionTreeWidget<T> : DrawableComponent
    {
        private readonly CollisionTreeProducer<T> producer_;

        public CollisionTreeWidget(CollisionTreeProducer<T> collisionTreeProducer)
        {
            Validate.Hard.IsNotNullOrDefault(collisionTreeProducer,
                () => this.GetFormattedNullOrDefaultMessage(nameof(collisionTreeProducer)));
            producer_ = collisionTreeProducer;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            ISpatialTree<T> spatialTree = producer_();
            /* Draw each "collision rectangle" */
            List<DxRectangle> divisions = spatialTree.Divisions;
            foreach(var division in divisions)
            {
                spriteBatch.DrawBorder(division, 1, Color.DarkSlateBlue);
            }
            /* ...as well as each Node in the tree */
            List<DxRectangle> nodes = spatialTree.Nodes;
            foreach(var nodeBoundary in nodes)
            {
                spriteBatch.DrawBorder(nodeBoundary, 1, Color.Black);
            }
        }
    }
}