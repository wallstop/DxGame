﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Utils;

namespace DXGame.Core
{
    public class SortedList<T> : IList<T> where T : IComparable<T>
    {
        private readonly Comparer<T> comparer_;
        private readonly List<T> list_;

        public SortedList()
        {
            list_ = new List<T>();
            comparer_ = Comparer<T>.Default;
        }

        public SortedList(Comparer<T> comparer)
        {
            Validate.IsNotNull(comparer, $"Cannot create a SortedList for Type {typeof (T)} with a null Comparer");
            comparer_ = comparer;
        }

        public SortedList(int capacity)
        {
            list_ = new List<T>(capacity);
        }

        public SortedList(IEnumerable<T> collection)
            : this(collection?.Count() ?? 0)
        {
            var collectionAsArray = collection.ToArray();
            Array.Sort(collectionAsArray, comparer_);
            list_.AddRange(collectionAsArray);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            int index = list_.BinarySearch(item, comparer_);
            if (index < 0)
            {
                index = ~index;
            }
            list_.Insert(index, item);
        }

        public void Clear()
        {
            list_.Clear();
        }

        public bool Contains(T item)
        {
            int index = list_.BinarySearch(item, comparer_);
            return index >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list_.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return list_.Remove(item);
        }

        public int Count => list_.Count;
        public bool IsReadOnly => false;

        public int IndexOf(T item)
        {
            return list_.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Add(item);
        }

        public void RemoveAt(int index)
        {
            list_.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return list_[index]; }
            set { throw new ArgumentException($"Cannot call set on access operator of {GetType()}"); }
        }

        public void RemoveBelow(T item)
        {
            while (Count > 0 && comparer_.Compare(list_[0], item) < 0)
            {
                RemoveAt(0);
            }
        }

        public void RemoveAbove(T item)
        {
            while (Count > 0 && comparer_.Compare(list_[Count - 1], item) > 0)
            {
                RemoveAt(Count - 1);
            }
        }
    }
}