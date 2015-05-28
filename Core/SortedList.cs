using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXGame.Core
{
    public class SortedList<T> : ICollection<T> where T : IComparable<T>
    {
        private readonly List<T> list_;

        public SortedList()
        {
            list_ = new List<T>();
        }

        public SortedList(int capacity)
        {
            list_ = new List<T>(capacity);
        }

        public SortedList(IEnumerable<T> collection)
            : this(collection.Count())
        {
            var collectionAsArray = collection.ToArray();
            Array.Sort(collectionAsArray);
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
            int index = list_.BinarySearch(item);
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
            int index = list_.BinarySearch(item);
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

        public int Count
        {
            get { return list_.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return list_[index]; }
        }
    }
}