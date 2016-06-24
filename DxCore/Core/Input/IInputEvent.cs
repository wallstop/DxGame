namespace DxCore.Core.Input
{
    public interface IInputEvent<out T> : IMeasuredInputEvent
    {
        T Source { get; }
    }
}
