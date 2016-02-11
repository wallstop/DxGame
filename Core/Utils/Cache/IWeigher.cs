namespace DXGame.Core.Utils.Cache
{
    public interface IWeigher<in K, in V>
    {
        int Weigh(K key, V value);
    }
}