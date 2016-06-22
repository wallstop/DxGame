using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;
using DXGame.Core.Utils;

namespace DxCore.Core.DataStructures
{
    /**
        <summary>
            Simple FIFO based circular buffer. Overwrites old values.
        </summary>
    */

    [DataContract]
    [Serializable]
    public class CircularBuffer<T> : IEnumerable<T>
    {
        [DataMember] private T[] buffer_;

        [DataMember] private int position_;

        [DataMember]
        public int Capacity { get; }

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

        public T Peek()
        {
            return this[Count - 1];
        }

        private void BoundsCheck(int index)
        {
            if(Count < index || index < 0)
            {
                throw new IndexOutOfRangeException($"{index} is outside of bounds [0, {Count})");
            }
        }

        public CircularBuffer(int capacity)
        {
            Validate.Hard.IsTrue(0 < capacity, this.GetFormattedNullOrDefaultMessage(nameof(capacity)));
            Capacity = capacity;
            position_ = 0;
            buffer_ = new T[capacity];
        }

        public void Clear()
        {
            /* Simply reset state */
            Count = 0;
            position_ = 0;
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
    }
}