using System;
using System.Runtime.Serialization;

namespace DxCore.Core.DataStructures
{
    [Serializable]
    [DataContract]
    public class Tree<T> {}

    [Serializable]
    [DataContract]
    public class TreeNode<T>
    {
        [DataMember]
        public T Value { get; }

        internal TreeNode(T value)
        {
            Value = value;
        }
    }
}