using DxCore.Core.Primitives;

namespace DxCore.Core.Components.Advanced.Position
{
    public interface IPositional
    {
        DxVector2 WorldCoordinates { get; }
    }
}