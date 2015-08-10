using System;
using System.Collections;
using System.Collections.Generic;
using DXGame.Core.Wrappers;

namespace DXGame.Core.Utils.Distance
{
    internal class QuadTreeNode<T>
    {
        public QuadTreeNode<T> Left { get; }
        public QuadTreeNode<T> Right { get; }
        public IEnumerable<T> Points { get; }

        public bool Terminal => Left == null && Right == null;

        public QuadTreeNode(QuadTreeNode<T> left, QuadTreeNode<T> right, IEnumerable<T> range)
        {
            Left = left;
            Right = right;
            Points = range;
        }
    }

    public class QuadTree<T> : ICollection<T>
    {
        private static readonly int DEFAULT_BUCKET_SIZE = 12;
        private Coordinate<T> coordinate_;
        private Distance distance_;

        public QuadTree(Coordinate<T> coordinate, IEnumerable<T> points)
            : this(coordinate, DxVector2.DistanceBetweenSquared, points, DEFAULT_BUCKET_SIZE)
        {
        }

        public QuadTree(Coordinate<T> coordinate, IEnumerable<T> points, int bucketSize)
            : this(coordinate, DxVector2.DistanceBetweenSquared, points, bucketSize)
        {
        }

        public QuadTree(Coordinate<T> coordinate, Distance distance, IEnumerable<T> points, int bucketSize)
        {
            Validate.IsTrue(bucketSize > 0, $"Cannot create a {GetType()} with a {nameof(bucketSize)} of {bucketSize}");
            Validate.IsNotNull(coordinate, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(coordinate)));
            Validate.IsNotNull(distance, StringUtils.GetFormattedNullOrDefaultMessage(this, nameof(distance)));
            coordinate_ = coordinate;
            distance_ = distance;

            BuildTree(points);
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }

        private void BuildTree(IEnumerable<T> points)
        {
            List<int> a = new List<int>();
        }

        private static void Divide(List<T> points, int minIndex, int maxIndex, int bucketSize, QuadTreeNode<T> parent)
        {
            if (maxIndex - minIndex <= bucketSize)
            {

            }

        }
    }
}