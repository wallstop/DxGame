using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.Primitives;

namespace DxCore.Core.Utils.Distance
{
    [Serializable]
    [DataContract]
    public class EmptyTree<T> : ISpatialTree<T>
    {
        private static readonly Lazy<EmptyTree<T>> Singleton = new Lazy<EmptyTree<T>>(() => new EmptyTree<T>());

        public static EmptyTree<T> Instance => Singleton.Value;

        private EmptyTree()
        {
            // This page intentionally left blank
        }

        public List<T> Elements => Enumerable.Empty<T>().ToList();
        public List<DxRectangle> Nodes => Enumerable.Empty<DxRectangle>().ToList();
        public List<DxRectangle> Divisions => Enumerable.Empty<DxRectangle>().ToList();
        public List<T> InRange(DxRectangle range) => Enumerable.Empty<T>().ToList();

        public bool Closest(DxVector2 position, out T result)
        {
            result = default(T);
            return false;
        }
    }
}
