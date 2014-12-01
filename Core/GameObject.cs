using System.Collections.Generic;
using DXGame.Core.Components;

namespace DXGame.Core
{
    public abstract class GameObject
    {
        protected readonly UniqueId id_;

        protected List<Component> components_ = new List<Component>();
        protected List<DrawableComponent> drawableComponents_ = new List<DrawableComponent>();
        protected List<UpdateableComponent> updateableComponents_ = new List<UpdateableComponent>();

        public UniqueId Id
        {
            get { return id_; }
        }

        public bool AttachComponent(Component component)
        {
            return addTo(components_, component);
        }

        public bool AttachUpdateableComponent(UpdateableComponent component)
        {
            return addTo(updateableComponents_, component);
        }

        public bool AttachDrawableComponent(DrawableComponent component)
        {
            return addTo(drawableComponents_, component);
        }

        private static bool addTo<T>(List<T> list, T element)
        {
            bool alreadyContains = list.Contains(element);
            if (!alreadyContains)
                list.Add(element);
            return alreadyContains;
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

        public GameObject WithUpdateableComponent(UpdateableComponent component)
        {
            AttachUpdateableComponent(component);
            return this;
        }

        public GameObject WithDrawableComponent(DrawableComponent component)
        {
            AttachDrawableComponent(component);
            return this;
        }

        public bool Update()
        {
            bool allUpdatesSucceeded = true;
            foreach (UpdateableComponent component in updateableComponents_)
            {
                bool updateSucceeded = component.Update();
                allUpdatesSucceeded = allUpdatesSucceeded && updateSucceeded;
            }
            return allUpdatesSucceeded;
        }
    }
}