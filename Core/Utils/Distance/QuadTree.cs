using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Primitives;

namespace DXGame.Core.Utils.Distance
{
    [DataContract]
    [Serializable]
    internal class QuadTreeNode<T>
    {
        private static readonly int NUM_CHILDREN = 4;

        [DataMember]
        public List<QuadTreeNode<T>> Children { get; }
        [DataMember]
        public DxRectangle Boundary { get; }
        [DataMember]
        public List<T> Points { get; }
        [DataMember]
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

    [DataContract]
    [Serializable]
    public class QuadTree<T> : ISpatialTree<T>
    {
        private static readonly int DEFAULT_BUCKET_SIZE = 12;
        [DataMember]
        private readonly DxRectangle boundary_;
        [DataMember]
        private readonly Coordinate<T> coordinate_;
        [DataMember]
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
            head_ = new QuadTreeNode<T>(boundary, coordinate_, points.ToList(), bucketSize);
        }

        // TODO: Simplifiy all these trees into a better structure (too much copy paste) 
        public List<T> Elements {
            get {

                Queue<QuadTreeNode<T>> nodesToVisit = new Queue<QuadTreeNode<T>>();
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

                    foreach (var node in currentNode.Children)
                    {
                        nodesToVisit.Enqueue(node);
                    }
                } while (nodesToVisit.Any());

                return elements;
            }
        }
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

        public List<T> InRange(IShape range)
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
                    foreach (var child in currentNode.Children)
                    {
                        nodesToVisit.Enqueue(child);
                    }
                }
            } while (nodesToVisit.Any());
            return elementsInRange;
        }

        /**

            <summary>
                Provides an approximate nearest-neighbor element for the specified point
            </summary>
        */

        public Optional<T> Closest(DxVector2 position)
        {
            // wtf resharper impure method?
            if (!boundary_.Contains(position))
            {
                return Optional<T>.Empty;
            }

            var currentNode = head_;
            
            /* Drill down until we're at the terminal node that contains our target */
            while (!currentNode.Terminal)
            {
                var children = currentNode.Children;
                var closestChild = children.FirstOrDefault(child => child.Boundary.Contains(position));
                if (ReferenceEquals(closestChild, null))
                {
                    break;
                }
                currentNode = closestChild;
            }
            Validate.IsTrue(currentNode.Terminal);

            /* We need to find all nodes that are above and beyond us (http://gamedev.stackexchange.com/questions/27264/how-do-i-optimize-searching-for-the-nearest-point). IE, two layers out, one layer in. */
            DxRectangle searchBoundary = currentNode.Boundary;
            searchBoundary.X -= searchBoundary.Width;
            searchBoundary.Y -= searchBoundary.Height;
            searchBoundary.Width *= 3;
            searchBoundary.Height *= 3;
            List<T> points = InRange(searchBoundary);
            if (!points.Any())
            {
                return Optional<T>.Empty;
            }

            /* Points? Walk the list once to find the closest one */
            var closest = points[0];
            var smallestMagnitude = (position - coordinate_(closest)).MagnitudeSquared;
            foreach (var point in points)
            {
                var magnitude = (position - coordinate_(point)).MagnitudeSquared;
                if (magnitude < smallestMagnitude)
                {
                    closest = point;
                    smallestMagnitude = magnitude;
                }
            }

            return new Optional<T>(closest);
        }

        public List<T> InRange(DxVector2 point, float radius)
        {
            return InRange(new DxRectangle(point.X - radius, point.Y - radius, radius, radius));
        }
    }
}