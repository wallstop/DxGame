using System;
using System.Collections.Generic;
using DxCore.Core.Primitives;
using DxCore.Core.Utils;

namespace DxCore.Core
{
    public enum UpdatePriority
    {
        GameObject = -150, /* Game Objects need to update before all else so their message queues are swapped */
        Input = -130,
        Physics = -100,
        High = 1,
        Normal = 5,
        Low = 10,
        State = 100
    }

    public interface IProcessable : IComparable<IProcessable>
    {
        UpdatePriority UpdatePriority { get; }
        void Process(DxGameTime gameTime);
    }

    public static class Processable
    {
        public static IComparer<IProcessable> DefaultComparer { get; } = new LambdaUtils.LambdaComparer<IProcessable>(
            (first, second) =>
                ((int?) first?.UpdatePriority ?? int.MinValue).CompareTo(((int?) second?.UpdatePriority ?? int.MinValue)))
            ;
    }
}