namespace DXGame.Core
{
    public class Component
    {
        protected UniqueId id_;

        public UniqueId Id
        {
            get { return id_; }
        }

        public virtual bool Update()
        {
            return true;
        }
    }
}