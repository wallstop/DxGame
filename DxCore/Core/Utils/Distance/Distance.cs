using DxCore.Core.Primitives;

namespace DxCore.Core.Utils.Distance
{
    public delegate DxVector2 Coordinate<in T>(T point);

    public delegate DxRectangle BoundingBox<in T>(T thing);

    public delegate double Distance(DxVector2 lhs, DxVector2 rhs);
}