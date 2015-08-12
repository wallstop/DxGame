using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Utils.Distance
{
    /* 
        Children are defined as "being of the cartesian quadrant" 
        https://en.wikipedia.org/wiki/Quadrant_%28plane_geometry%29

        Where the quadrants are

        II  | I
        ____|___
            |
        III | IV
    */

    internal class QuadTreeNode<T>
    {
        private static readonly int NUM_CHILDREN = 4;
        public List<QuadTreeNode<T>> Children { get; }
        public DxRectangle Boundary { get; }
        public IEnumerable<T> Points { get; }
        // TODO: Fix
        public bool Terminal { get; }

        public QuadTreeNode(DxRectangle boundary, Coordinate<T> coordinate, IEnumerable<T> pointsInSpace, int bucketSize)
        {
            Boundary = boundary;
            Terminal = pointsInSpace.Count() <= bucketSize;
            Children = new List<QuadTreeNode<T>>(NUM_CHILDREN);
            if (Terminal)
            {
                Points = pointsInSpace;
                return;
            }

            DxRectangle[] quadrants =
            {
                boundary.QuadrantOne, boundary.QuadrantTwo, boundary.QuadrantThree,
                boundary.QuadrantFour
            };

            /* Make sure we don't place elements into multiple regions */
            HashSet<T> placedElements = new HashSet<T>();
            foreach (DxRectangle quadrant in quadrants)
            {
                var node = new QuadTreeNode<T>(quadrant, coordinate,
                    pointsInSpace.Where(element => quadrant.Contains(coordinate(element)) && placedElements.Add(element)),
                    bucketSize);
                Children.Add(node);
            }
        }
    }

    public class QuadTree<T>
    {
        private static readonly int DEFAULT_BUCKET_SIZE = 12;
        private readonly DxRectangle boundary_;
        private readonly Coordinate<T> coordinate_;
        private readonly QuadTreeNode<T> head_;

        public QuadTree(Coordinate<T> coordinate, DxRectangle boundary, IEnumerable<T> points)
            : this(coordinate, boundary, points, DEFAULT_BUCKET_SIZE)
        {
        }

        public QuadTree(Coordinate<T> coordinate, DxRectangle boundary, IEnumerable<T> points, int bucketSize)
        {
            Validate.IsTrue(bucketSize > 0, $"Cannot create a {GetType()} with a {nameof(bucketSize)} of {bucketSize}");
            Validate.IsNotNull(coordinate, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(coordinate)));
            coordinate_ = coordinate;
            boundary_ = boundary;
            head_ = new QuadTreeNode<T>(boundary, coordinate_, points, bucketSize);
        }

        public List<T> InRange(DxRectangle range)
        {
            if (!range.Intersects(boundary_))
            {
                return Enumerable.Empty<T>().ToList();
            }

            Queue<QuadTreeNode<T>> nodesToVisit = new Queue<QuadTreeNode<T>>();
            nodesToVisit.Enqueue(head_);

            List<T> elementsInRange = new List<T>();
            while (nodesToVisit.Any())
            {
                QuadTreeNode<T> currentNode = nodesToVisit.Dequeue();
                if (!currentNode.Boundary.Intersects(range))
                {
                    continue;
                }

                if (currentNode.Terminal)
                {
                    elementsInRange.AddRange(currentNode.Points.Where(element => range.Contains(coordinate_(element))));
                }
                else
                {
                    foreach (var child in currentNode.Children)
                    {
                        nodesToVisit.Enqueue(child);
                    }
                }
            }
            return elementsInRange;
        }
    }
}