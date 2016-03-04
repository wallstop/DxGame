using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DXGame.Core.Utils
{
    // TODO: Expand, docs, implement queue-type interface?
    [DataContract]
    [Serializable]
    public class FixedSizedBuffer<T>
    {
        [DataMember] private readonly List<T> backingList_;

        [DataMember] private int offset_;

        [DataMember] public int Size { get; private set; }

        public T this[int position]
        {
            get { return backingList_[(CurrentOffset + position) % backingList_.Capacity]; }
            set { backingList_[(CurrentOffset + position) % backingList_.Capacity] = value; }
        }

        private int CurrentOffset => (offset_ - 1 + backingList_.Capacity) % backingList_.Capacity;

        [DataMember]
        public int Limit { get; }

        public FixedSizedBuffer(int limit)
        {
            Validate.IsTrue(limit > 0, $"Cannot create a {typeof(FixedSizedBuffer<T>)} with a limit of {limit}");
            Limit = limit;
            backingList_ = new List<T>(limit);
            for(int i = 0; i < limit; ++i)
            {
                backingList_.Add(default(T));
            }
            offset_ = 0;
        }

        public void Buffer(T value)
        {
            if(Size < backingList_.Capacity)
            {
                ++Size;
            }
            backingList_[offset_] = value;
            offset_.WrappedAdd(1, backingList_.Capacity);
        }
    }
}