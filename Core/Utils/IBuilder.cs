namespace DXGame.Core.Utils
{
    public interface IBuilder<out T>
    {
        T Build();
    }
}