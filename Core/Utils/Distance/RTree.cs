﻿using System;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Utils.Distance
{
    /* 
        TODO: Refactor these trees into some kind of interface 
    */

    internal class RTreeNode<T>
    {
        public DxRectangle Boundary { get; }
        public bool Terminal { get; }
        public List<T> Rectangles { get; }
        public List<RTreeNode<T>> Children { get; }

        public RTreeNode(BoundingBox<T> boundingBox, List<T> rectangles, int bucketSize, int branchFactor)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            foreach (T rectangularObject in rectangles)
            {
                var rectangle = boundingBox(rectangularObject);
                minX = Math.Min(minX, rectangle.X);
                maxX = Math.Max(maxX, rectangle.X);
                minY = Math.Min(minY, rectangle.Y);
                maxY = Math.Max(maxY, rectangle.Y);
            }

            Boundary = new DxRectangle(minX, minY, (maxX - minX), (maxY - minY));
            Terminal = rectangles.Count <= bucketSize;
            if (Terminal)
            {
                Rectangles = rectangles;
                return;
            }

            Children = new List<RTreeNode<T>>(branchFactor);

            /*
                http://www.dtic.mil/get-tr-doc/pdf?AD=ADA324493
                var targetSize = rectangles.Count / (double) branchFactor;
                P = branchFactor;
                S = Math.sqrt(P);
                N = targetSize;
                
                Ugh.
            */
            double targetSize = rectangles.Count / (double) branchFactor;
            int intTargetSize = (int) Math.Ceiling(targetSize);
            var P = branchFactor;
            var S = Math.Sqrt(P);
            var N = targetSize;
            double slicesPerAxis = Math.Sqrt(branchFactor);
            int rectanglesPerPagePerAxis = (int) (slicesPerAxis * targetSize);
            rectangles.Sort((box1, box2) => (boundingBox(box1).Center.X.CompareTo(boundingBox(box2).Center.X)));
            IEnumerable<List<T>> partitionedByX = rectangles.Partition(rectanglesPerPagePerAxis);
            foreach (var xSlice in partitionedByX)
            {
                xSlice.Sort((box1, box2) => (boundingBox(box1).Center.Y.CompareTo(boundingBox(box2).Center.Y)));
                IEnumerable<List<T>> partitionedByY = xSlice.Partition(intTargetSize);
                foreach (var ySlice in partitionedByY)
                {
                    var node = new RTreeNode<T>(boundingBox, ySlice, bucketSize, branchFactor);
                    Children.Add(node);
                }
            }
        }
    }

    public class RTree<T>
    {
        private static readonly int DEFAULT_BUCKET_SIZE = 10;
        private static readonly int DEFAULT_BRANCH_FACTOR = 4;
        private readonly DxRectangle boundary_;
        private readonly BoundingBox<T> boundingBox_;
        private readonly RTreeNode<T> head_;

        public RTree(BoundingBox<T> boundingBox, List<T> rectangles)
            : this(boundingBox, rectangles, DEFAULT_BUCKET_SIZE, DEFAULT_BRANCH_FACTOR)
        {
        }

        public RTree(BoundingBox<T> boundingBox, List<T> rectangles, int bucketSize,
            int branchFactor)
        {
            Validate.IsTrue(bucketSize > 0, $"Cannot create a {GetType()} with a {nameof(bucketSize)} of {bucketSize}");
            Validate.IsNotNull(boundingBox, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(boundingBox)));
            boundingBox_ = boundingBox;
            head_ = new RTreeNode<T>(boundingBox, rectangles, bucketSize, branchFactor);
            boundary_ = head_.Boundary;
        }

        public List<T> InRange(DxRectangle range)
        {
            if (!range.Intersects(boundary_))
            {
                return Enumerable.Empty<T>().ToList();
            }

            Queue<RTreeNode<T>> nodesToVisit = new Queue<RTreeNode<T>>();
            nodesToVisit.Enqueue(head_);

            List<T> elementsInRange = new List<T>();
            do
            {
                RTreeNode<T> currentNode = nodesToVisit.Dequeue();
                if (!currentNode.Boundary.Intersects(range))
                {
                    continue;
                }

                if (currentNode.Terminal)
                {
                    elementsInRange.AddRange(
                        currentNode.Rectangles.Where(element => range.Intersects(boundingBox_(element))));
                }
                else
                {
                    foreach (var child in currentNode.Children)
                    {
                        nodesToVisit.Enqueue(child);
                    }
                }
            } while (nodesToVisit.Any());
            return elementsInRange;
        }
    }
}