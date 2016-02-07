using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Utils.Distance
{
    public delegate float Axis<in T>(T point);

    [DataContract]
    [Serializable]
    internal class KDTreeNode<T>
    {
        [DataMember]
        public KDTreeNode<T> Left { get; }
        [DataMember]
        public KDTreeNode<T> Right { get; }
        [DataMember]
        public DxRectangle Boundary { get; }
        [DataMember]
        public List<T> Points { get; }
        [DataMember]
        public bool Terminal { get; }

        public KDTreeNode(DxRectangle boundary, Coordinate<T> coordinate, List<T> pointsInSpace, int bucketSize,
            bool isXAxis)
        {
            Boundary = boundary;
            Terminal = pointsInSpace.Count() <= bucketSize;
            if (Terminal)
            {
                Points = pointsInSpace;
                return;
            }

            int cutoff = pointsInSpace.Count / 2;

            Axis<T> pointFunction;
            DxRectangle[] newBoundary;
            if (isXAxis)
            {
                pointFunction = (point => coordinate(point).X);
                var halfWidth = boundary.Width / 2;
                newBoundary = new[]
                {
                    new DxRectangle(boundary.X, boundary.Y, halfWidth, boundary.Height),
                    new DxRectangle(boundary.X + halfWidth, boundary.Y, halfWidth, boundary.Height)
                };
            }
            else
            {
                pointFunction = (point => coordinate(point).Y);
                var halfHeight = boundary.Height / 2;
                newBoundary = new[]
                {
                    new DxRectangle(boundary.X, boundary.Y, boundary.Width, halfHeight),
                    new DxRectangle(boundary.X, boundary.Y + halfHeight, boundary.Width, halfHeight)
                };
            }
            pointsInSpace.Sort((point1, point2) => pointFunction(point1).CompareTo(pointFunction(point2)));

            List<T> left = new List<T>(pointsInSpace.Take(cutoff));
            List<T> right = new List<T>(pointsInSpace.Skip(cutoff).Take(pointsInSpace.Count - cutoff));
            Left = new KDTreeNode<T>(newBoundary[0], coordinate, left, bucketSize, !isXAxis);
            Right = new KDTreeNode<T>(newBoundary[1], coordinate, right, bucketSize, !isXAxis);
        }
    }

    [DataContract]
    [Serializable]
    public class KDTree<T> : ISpatialTree<T>
    {
        private static readonly int DEFAULT_BUCKET_SIZE = 12;
        [DataMember]
        private readonly DxRectangle boundary_;
        [DataMember]
        private readonly Coordinate<T> coordinate_;
        [DataMember]
        private readonly KDTreeNode<T> head_;

        public List<T> Elements
        {
            get {
                Queue<KDTreeNode<T>> nodesToVisit = new Queue<KDTreeNode<T>>();
                nodesToVisit.Enqueue(head_);
                var elements = new List<T>();

                do
                {
                    var currentNode = nodesToVisit.Dequeue();
                    if (currentNode.Terminal)
                    {
                        elements.AddRange(currentNode.Points);
                        continue;
                    }

                    nodesToVisit.Enqueue(currentNode.Left);
                    nodesToVisit.Enqueue(currentNode.Right);

                } while (nodesToVisit.Any());
                return elements;
            }
        }

        public List<DxRectangle> Nodes => Divisions; 

        public List<DxRectangle> Divisions
        {
            get
            {
                Queue<KDTreeNode<T>> nodesToVisit = new Queue<KDTreeNode<T>>();
                nodesToVisit.Enqueue(head_);

                List<DxRectangle> quadrants = new List<DxRectangle>();
                do
                {
                    KDTreeNode<T> currentNode = nodesToVisit.Dequeue();
                    quadrants.Add(currentNode.Boundary);
                    if (currentNode.Terminal)
                    {
                        continue;
                    }

                    nodesToVisit.Enqueue(currentNode.Left);
                    nodesToVisit.Enqueue(currentNode.Right);
                } while (nodesToVisit.Any());

                return quadrants;
            }
        }

        public KDTree(Coordinate<T> coordinate, DxRectangle boundary, List<T> points)
            : this(coordinate, boundary, points, DEFAULT_BUCKET_SIZE)
        {
        }

        public KDTree(Coordinate<T> coordinate, DxRectangle boundary, List<T> points, int bucketSize)
        {
            Validate.IsTrue(bucketSize > 0, $"Cannot create a {GetType()} with a {nameof(bucketSize)} of {bucketSize}");
            Validate.IsNotNull(coordinate, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(coordinate)));
            coordinate_ = coordinate;
            boundary_ = boundary;
            head_ = new KDTreeNode<T>(boundary, coordinate_, points, bucketSize, true);
        }

        public List<T> InRange(DxVector2 point, float radius)
        {
            return InRange(new DxRectangle(point.X - radius, point.Y - radius, radius, radius));
        }

        public List<T> InRange(IShape range)
        {
            if (!range.Intersects(boundary_))
            {
                return Enumerable.Empty<T>().ToList();
            }

            Queue<KDTreeNode<T>> nodesToVisit = new Queue<KDTreeNode<T>>();
            nodesToVisit.Enqueue(head_);

            List<T> elementsInRange = new List<T>();
            do
            {
                KDTreeNode<T> currentNode = nodesToVisit.Dequeue();
                if (!range.Intersects(currentNode.Boundary))
                {
                    continue;
                }

                if (currentNode.Terminal)
                {
                    elementsInRange.AddRange(currentNode.Points.Where(element => range.Contains(coordinate_(element))));
                }
                else
                {
                    nodesToVisit.Enqueue(currentNode.Left);
                    nodesToVisit.Enqueue(currentNode.Right);
                }
            } while (nodesToVisit.Any());
            return elementsInRange;
        }

        public Optional<T> Closest(DxVector2 position)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
