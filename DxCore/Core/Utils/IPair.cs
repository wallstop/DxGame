namespace DxCore.Core.Utils
{
    public interface IPair<T, U>
    {
        T Key { get; }
        U Value { get; }
    }
}
