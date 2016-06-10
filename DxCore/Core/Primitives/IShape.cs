namespace DxCore.Core.Primitives
{
    public interface IShape
    {
        bool Contains(DxVector2 point);
        bool Intersects(DxRectangle box);
    }
}
