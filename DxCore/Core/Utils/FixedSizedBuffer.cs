using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils
{
    // TODO: Expand, docs, implement queue-type interface?
    [DataContract]
    [Serializable]
    public class FixedSizedBuffer<T>
    {
        [DataMember] private readonly List<T> backingList_;

        public int Count => backingList_.Count;

        public T this[int position]
        {
            get { return backingList_[position]; }
            set { backingList_[position] = value; }
        }

        [DataMember]
        public int Limit { get; private set; }

        public FixedSizedBuffer(int limit)
        {
            Validate.Validate.Hard.IsTrue(limit > 0, $"Cannot create a {typeof(FixedSizedBuffer<T>)} with a limit of {limit}");
            Limit = limit;
            backingList_ = new List<T>(limit);
        }

        public void Buffer(T value)
        {
            if(backingList_.Count == Limit)
            {
                for(int i = 0; i < Limit - 1; ++i)
                {
                    backingList_[i] = backingList_[i + 1];
                }

                backingList_[Limit - 1] = value;
                return;
            }
            backingList_.Add(value);
        }
    }
}