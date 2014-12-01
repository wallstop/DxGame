using System.Collections.Generic;

namespace DXGame.Core
{
    public abstract class GameObject
    {
        protected readonly UniqueId id_;

        protected List<Component> components_ = new List<Component>();

        public UniqueId Id
        {
            get { return id_; }
        }

        public bool AttachComponent(Component component)
        {
            bool alreadyContains = components_.Contains(component);
            if (!alreadyContains)
                components_.Add(component);
            return !alreadyContains;
        }

        /*
            Allows for Constructor chaining.
            GameObject object = new GameObject().WithComponent(a).WithComponent(b) ...;
        */

        public GameObject WithComponent(Component component)
        {
            AttachComponent(component);
            return this;
        }

        public bool Update()
        {
            bool allUpdatesSucceeded = true;
            foreach (Component component in components_)
            {
                bool updateSucceeded = component.Update();
                allUpdatesSucceeded = allUpdatesSucceeded && updateSucceeded;
            }
            return allUpdatesSucceeded;
        }
    }
}