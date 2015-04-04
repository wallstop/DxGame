namespace DXGame.Core
{
    public interface IPersistable<out T>
    {
        T Load();
        void Save();
    }
}