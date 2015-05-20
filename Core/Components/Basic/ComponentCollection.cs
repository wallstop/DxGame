using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Utils;

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
        private readonly List<Component> components_ = new List<Component>();
        private readonly IComparer<DrawableComponent> drawPriorityComparer_ = new DrawPriorityComparer();
        private readonly IComparer<Component> updatePriorityComparer_ = new UpdatePriorityComparer();

        public int Count
        {
            get { return components_.Count; }
        }

        public IEnumerable<Component> Components()
        {
            return components_;
        }

        public IEnumerable<DrawableComponent> Drawables()
        {
            var drawableComponents = components_.OfType<DrawableComponent>().ToList();
            drawableComponents.Sort(drawPriorityComparer_);
            return drawableComponents;
        }

        public void Add(Component component)
        {
            components_.Add(component);
            /* 
                Right now, this is "good enough". I haven't been able to find something like http://en.cppreference.com/w/cpp/container/priority_queue in C#. 
                We take the naive approach of re-sorting the entire collection whenever an element is added. This is expensive, but without a custom container,
                unavoidable.

                If this is too expensive, swap out list for a better, custom container (btree? red black tree?) that will be more performant.
            */
            components_.Sort(updatePriorityComparer_);
        }

        public void Remove(Component component)
        {
            components_.Remove(component);
        }
    }
}