using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Utils.Distance
{
    internal class QuadTreeNode<T>
    {
        private static readonly int NUM_CHILDREN = 4;
        public List<QuadTreeNode<T>> Children { get; }
        public DxRectangle Boundary { get; }
        public List<T> Points { get; }
        public bool Terminal { get; }

        public QuadTreeNode(DxRectangle boundary, Coordinate<T> coordinate, List<T> pointsInSpace, int bucketSize)
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

            foreach (DxRectangle quadrant in quadrants)
            {
                var pointsInRange =
                    pointsInSpace.Where(element => quadrant.Contains(coordinate(element))).ToList();
                var node = new QuadTreeNode<T>(quadrant, coordinate, pointsInRange, bucketSize);
                Children.Add(node);
            }
        }
    }

    public class QuadTree<T> : ICollisionTree<T>
    {
        private static readonly int DEFAULT_BUCKET_SIZE = 12;
        private readonly DxRectangle boundary_;
        private readonly Coordinate<T> coordinate_;
        private readonly QuadTreeNode<T> head_;

        public List<DxRectangle> Nodes => Divisions; 

        public List<DxRectangle> Divisions
        {
            get
            {
                Queue<QuadTreeNode<T>> nodesToVisit = new Queue<QuadTreeNode<T>>();
                nodesToVisit.Enqueue(head_);

                List<DxRectangle> quadrants = new List<DxRectangle>();
                do
                {
                    QuadTreeNode<T> currentNode = nodesToVisit.Dequeue();
                    quadrants.Add(currentNode.Boundary);
                    if (currentNode.Terminal)
                    {
                        continue;
                    }

                    foreach (var node in currentNode.Children)
                    {
                        nodesToVisit.Enqueue(node);
                    }
                } while (nodesToVisit.Any());

                return quadrants;
            }
        }

        public QuadTree(Coordinate<T> coordinate, DxRectangle boundary, List<T> points)
            : this(coordinate, boundary, points, DEFAULT_BUCKET_SIZE)
        {
        }

        public QuadTree(Coordinate<T> coordinate, DxRectangle boundary, List<T> points, int bucketSize)
        {
            Validate.IsTrue(bucketSize > 0, $"Cannot create a {GetType()} with a {nameof(bucketSize)} of {bucketSize}");
            Validate.IsNotNull(coordinate, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(coordinate)));
            coordinate_ = coordinate;
            boundary_ = boundary;
            head_ = new QuadTreeNode<T>(boundary, coordinate_, points, bucketSize);
        }

        public List<T> InRange(DxVector2 point, float radius)
        {
            return InRange(new DxRectangle(point.X - radius, point.Y - radius, radius, radius));
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
            do
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
            } while (nodesToVisit.Any());
            return elementsInRange;
        }
    }
}