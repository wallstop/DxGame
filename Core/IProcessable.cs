using System;
using System.Collections.Generic;
using DXGame.Core.Utils;
using DXGame.Core.Wrappers;

namespace DXGame.Core
{
    public enum UpdatePriority
    {
        GAME_OBJECT = -150, /* Game Objects need to update before all else so their message queues are swapped */
        INPUT = -130,
        PHYSICS = -100,
        STATE = 0,
        HIGH = 1,
        NORMAL = 5,
        LOW = 10
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