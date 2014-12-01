namespace DXGame.Core
{
    public abstract class Component
    {
        protected UniqueId id_;

        public UniqueId Id
        {
            get { return id_; }
        }
    }
}