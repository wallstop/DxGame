using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace DxCore.Core.Utils.Cache
{
    /**
        <summary>
            This queue simply discards whatever is thrown into it. It's a stateless, fast, immutable, empty list. 
            Also threadsafe.

            Extremely useful for when you NEED a queue but don't want one that actually does anything.

            Extremely useful. All the time. Use it always.
        </summary>

        Why aren't you using it?
    */
    [Serializable]
    [DataContract]
    public sealed class DiscardingQueue<T> : IProducerConsumerCollection<T>
    {
        private static readonly Lazy<DiscardingQueue<T>> Singleton =
            new Lazy<DiscardingQueue<T>>(() => new DiscardingQueue<T>());

        private readonly ReadOnlyCollection<T> absolutelyNothing_ = new ReadOnlyCollection<T>(new List<T>(0));

        public static DiscardingQueue<T> Instance => Singleton.Value;

        private DiscardingQueue() {}

        public IEnumerator<T> GetEnumerator()
        {
            return absolutelyNothing_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            // Fastest copy implementation in the west
        }

        public int Count => 0;
        public object SyncRoot => absolutelyNothing_;
        public bool IsSynchronized => true;

        public void CopyTo(T[] array, int index)
        {
            // Second-fastest copy implementation in the west
        }

        public bool TryAdd(T item)
        {
            return true;
        }

        public bool TryTake(out T item)
        {
            item = default(T);
            return false;
        }

        public T[] ToArray()
        {
            return absolutelyNothing_.ToArray();
        }
    }
}