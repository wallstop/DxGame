namespace DxCore.Core.Utils
{
    public interface IBuilder<out T>
    {
        T Build();
    }
}