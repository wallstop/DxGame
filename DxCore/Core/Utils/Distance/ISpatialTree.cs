using System.Collections.Generic;
using DxCore.Core.Primitives;

namespace DxCore.Core.Utils.Distance
{
    /**

        <summary>
            TODO
        </summary>
    */
    public interface ISpatialTree<T>
    {
        List<T> Elements { get; }
        List<DxRectangle> Nodes { get; }
        List<DxRectangle> Divisions { get; }
        List<T> InRange(IShape range);
        // TODO: Make Optional
        bool Closest(DxVector2 position, out T result);
    }
}
