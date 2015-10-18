using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Utils.Distance
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
