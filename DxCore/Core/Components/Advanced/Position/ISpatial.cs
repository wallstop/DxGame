using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Position
{
    public interface ISpatial : IPositional
    {
        DxRectangle Space { get; }
    }
}