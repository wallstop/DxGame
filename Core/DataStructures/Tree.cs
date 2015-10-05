using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core.DataStructures
{
    [Serializable]
    [DataContract]
    public class Tree<T>
    {
    }

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
