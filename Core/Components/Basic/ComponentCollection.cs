using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXGame.Core.Components.Utils;

namespace DXGame.Core.Components.Basic
{
    public class ComponentCollection : IEnumerable<Component>, IEnumerable<DrawableComponent>
    {
        private readonly List<Component> components_ = new List<Component>();
        private readonly IComparer<Component> updatePriorityComparer_ = new UpdatePriorityComparer();
        private readonly IComparer<DrawableComponent> drawPriorityComparer_ = new DrawPriorityComparer();

        public int Count
        {
            get { return components_.Count; }
        }

        IEnumerator<DrawableComponent> IEnumerable<DrawableComponent>.GetEnumerator()
        {
            var drawableComponents = ((List<DrawableComponent>) (components_.Where(n => (n is DrawableComponent))));
            drawableComponents.Sort(drawPriorityComparer_);
            return drawableComponents.GetEnumerator();
        }

        IEnumerator<Component> IEnumerable<Component>.GetEnumerator()
        {
            return ((IEnumerable<Component>) components_).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
        }

        public void Add(Component component)
        {
            components_.Add(component);
            components_.Sort(updatePriorityComparer_);
        }

        public void Remove(Component component)
        {
            components_.Remove(component);
        }
    }
}
