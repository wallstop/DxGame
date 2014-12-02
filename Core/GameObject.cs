using System.Collections.Generic;
using DXGame.Core.Components;

namespace DXGame.Core
{
    public class GameObject
    {
        private readonly List<DrawableComponent> drawables_ = new List<DrawableComponent>();
        private readonly UniqueId id_;
        private readonly List<UpdateableComponent> updateables_ = new List<UpdateableComponent>();

        public UniqueId Id
        {
            get { return id_; }
        }

        public List<DrawableComponent> Drawables
        {
            get { return drawables_; }
        }

        public List<UpdateableComponent> Updateables
        {
            get { return updateables_; }
        }

        protected GameObject AttachComponent(Component component)
        {
            var drawable = component as DrawableComponent;
            if (drawable != null)
            {
                drawables_.Add(drawable);
                return this;
            }

            var updateable = component as UpdateableComponent;
            if (updateable != null)
            {
                updateables_.Add(updateable);
                return this;
            }

            return this;
        }

        public GameObject AttachComponents(params Component[] components)
        {
            foreach (Component component in components)
            {
                AttachComponent(component);
            }
            return this;
        }
    }
}