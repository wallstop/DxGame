using DxCore.Core.Primitives;
using EmptyKeys.UserInterface;

namespace AnimationEditorLibrary.EmptyKeysLib
{
    public static class PointFExtensions
    {
        public static DxVector2 ToDxVector2(this PointF point)
        {
            return new DxVector2(point.X, point.Y);
        }
    }
}