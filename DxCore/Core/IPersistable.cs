namespace DxCore.Core
{
    public interface IPersistable<out T>
    {
        T Load(string fileName);
        void Save(string fileName);
    }
}