namespace DXGame.Core.Utils.Cache
{
    public interface IRemovalListener<K, V>
    {
        void OnRemoval(RemovalNotification<K, V> removalNotification);
    }

    public class NullRemovalListener<K, V> : IRemovalListener<K, V>
    {
        public static NullRemovalListener<K, V> Instance { get; } = new NullRemovalListener<K, V>();

        private NullRemovalListener() {}

        public void OnRemoval(RemovalNotification<K, V> removalNotification)
        {
            // No-op
        }
    }
}
