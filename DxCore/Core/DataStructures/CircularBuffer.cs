using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.DataStructures
{
    /**
        <summary>
            Simple FIFO based circular buffer. Overwrites old values.
        </summary>

        <code>
            // Indexing is done backwards-like. ie:
            CircularBuffer buffer = new CircularBuffer<int>(50);
            buffer.Add(13); // [13, 0, 0, 0, ...]
            buffer.Add(14); // [14, 13, 0, 0, ...]
        </code>
    */

    [DataContract]
    [Serializable]
    public class CircularBuffer<T> : IEnumerable<T>
    {
        [DataMember] private T[] buffer_;

        [DataMember] private int position_;

        [DataMember]
        public int Capacity { get; private set; }

        [DataMember]
        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                BoundsCheck(index);
                return buffer_[(position_ - 1 + Capacity - index) % Capacity];
            }
            set
            {
                BoundsCheck(index);
                buffer_[(position_ - 1 + Capacity - index) % Capacity] = value;
            }
        }

        public CircularBuffer(int capacity)
        {
            Validate.Hard.IsPositive(capacity, () => this.GetFormattedNullOrDefaultMessage(nameof(capacity)));
            Capacity = capacity;
            position_ = 0;
            buffer_ = new T[capacity];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; ++i)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            buffer_[position_] = item;
            position_ = position_.WrappedAdd(1, Capacity);
            if(Count < Capacity)
            {
                ++Count;
            }
        }

        public void Clear()
        {
            /* Simply reset state */
            Count = 0;
            position_ = 0;
        }

        public bool Peek(out T value)
        {
            if(InBounds(0))
            {
                value = this[0];
                return true;
            }
            value = default(T);
            return false;
        }

        private void BoundsCheck(int index)
        {
            if(!InBounds(index))
            {
                throw new IndexOutOfRangeException($"{index} is outside of bounds [0, {Count})");
            }
        }

        private bool InBounds(int index)
        {
            return !((Count < index) || (index < 0));
        }
    }
}