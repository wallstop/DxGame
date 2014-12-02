namespace DXGame.Core
{
    public abstract class Component
    {
        protected UniqueId id_;
        protected GameObject parent_;

        public UniqueId Id
        {
            get { return id_; }
        }

        public GameObject Parent
        {
            get { return parent_; }
            set { parent_ = value; }
        }

        protected Component(GameObject parent = null)
        {
            parent_ = parent;
        }
    }
}