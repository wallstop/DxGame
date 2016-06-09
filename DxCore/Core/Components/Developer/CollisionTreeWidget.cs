﻿using System.Collections.Generic;
using DxCore.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Distance;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public override bool ShouldSerialize => false;

        public CollisionTreeWidget(CollisionTreeProducer<T> collisionTreeProducer)
        {
            Validate.IsNotNullOrDefault(collisionTreeProducer,
                StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(collisionTreeProducer)));
            producer_ = collisionTreeProducer;
        }

        public override void Draw(SpriteBatch spriteBatch, DxGameTime gameTime)
        {
            ISpatialTree<T> spatialTree = producer_();
            /* Draw each "collision rectangle" */
            List<DxRectangle> divisions = spatialTree.Divisions;
            foreach (var division in divisions)
            {
                spriteBatch.DrawBorder(division, 1, Color.DarkSlateBlue);
            }
            /* ...as well as each Node in the tree */
            List<DxRectangle> nodes = spatialTree.Nodes;
            foreach (var nodeBoundary in nodes)
            {
                spriteBatch.DrawBorder(nodeBoundary, 1, Color.Black);
            }
        }
    }
}