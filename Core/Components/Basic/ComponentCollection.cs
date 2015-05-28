using System.Collections.Generic;

namespace DXGame.Core.Components.Basic
{
    /**
    <summary>
        ComponentCollection servers as our drop-in replacement for XNA's Game's ComponentCollection class. We need a collection that
        * Prioritizes components based on Update Priority (for iterating during Update(...))
        * Prioritizes components based on Draw Priority (for iterating during Draw(...))
        * Features Add/Remove
    </summary>
    */

    public class ComponentCollection
    {
        // TODO: Figure out how to serialize (readonly propertie) / if we need to
        /* 
            Right now, this is "good enough". I haven't been able to find something like http://en.cppreference.com/w/cpp/container/priority_queue in C#. 
            We take the naive approach of re-sorting the entire collection whenever an element is added. This is expensive, but without a custom container,
            unavoidable.

            If this is too expensive, swap out list for a better, custom container (btree? red black tree?) that will be more performant.
            Right now it's pretty dumb, dunno about performant.
        */
        private readonly IList<Component> components_ = new SortedList<Component>();
        private readonly IList<DrawableComponent> drawableComponents_ = new SortedList<DrawableComponent>();
        public int Count => components_.Count;

        public IEnumerable<Component> Components()
        {
            // TODO: Immutability?
            return components_;
        }

        public IEnumerable<DrawableComponent> Drawables()
        {
            // TODO: Immutability?
            return drawableComponents_;
        }

        public void Add(Component component)
        {
            components_.Add(component);

            DrawableComponent drawable = component as DrawableComponent;
            if (drawable != null)
            {
                drawableComponents_.Add(drawable);
            }
        }

        public void Remove(Component component)
        {
            components_.Remove(component);

            DrawableComponent drawable = component as DrawableComponent;
            if (drawable != null)
            {
                drawableComponents_.Remove(drawable);
            }
        }
    }
}