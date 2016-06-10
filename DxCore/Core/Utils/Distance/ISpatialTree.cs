using System.Collections.Generic;
using DxCore.Core.Primitives;
using DXGame.Core.Utils;

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
        Optional<T> Closest(DxVector2 position);
    }
}
