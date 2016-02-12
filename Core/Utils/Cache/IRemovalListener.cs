namespace DXGame.Core.Utils.Cache
{
    public interface IRemovalListener<K, V>
    {
        void OnRemoval(RemovalNotification<K, V> removalNotification);
    }
}
