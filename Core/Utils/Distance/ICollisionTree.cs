using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DXGame.Core.Primitives;

namespace DXGame.Core.Utils.Distance
{
    public interface ICollisionTree<T>
    {
        List<DxRectangle> Nodes { get; }
        List<DxRectangle> Divisions { get; }
        List<T> InRange(IShape range);
    }
}
