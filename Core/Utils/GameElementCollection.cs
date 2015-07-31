using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXGame.Core.Utils
{
    /**
    <summary>
        GameElementCollection servers as our drop-in replacement for XNA's Game's ComponentCollection class. We need a collection that
        * Prioritizes components based on Update Priority (for iterating during Update(...))
        * Prioritizes components based on Draw Priority (for iterating during Draw(...))
        * Features Add/Remove
    </summary>
    */

    public class GameElementCollection : IEnumerable
        /* Should we deal with having this implement the ICollection interface? */
    {
        private readonly ICollection<IDrawable> drawables_ = new SortedList<IDrawable>(Drawable.DefaultComparer);
        /* 
            Right now, this is "good enough". I haven't been able to find something like http://en.cppreference.com/w/cpp/container/priority_queue in C#. 

            If this is too expensive, swap out list for a better, custom container (btree? red black tree?) that will be more performant.
            Right now it's pretty dumb, dunno about performant.
        */

        private readonly ICollection<IProcessable> processables_ =
            new SortedList<IProcessable>(Processable.DefaultComparer);

        public int Count => processables_.Count;
        public IEnumerable<IProcessable> Processables => processables_;
        public IEnumerable<IDrawable> Drawables => drawables_;

        public IEnumerator GetEnumerator()
        {
            // TODO: Make this not suck
            var allOwnedElements = new HashSet<object>();
            foreach (var processable in processables_.Where(processable => !allOwnedElements.Contains(processable)))
            {
                allOwnedElements.Add(processable);
            }
            foreach (var drawable in drawables_.Where(drawable => !allOwnedElements.Contains(drawable)))
            {
                allOwnedElements.Add(drawable);
            }

            return allOwnedElements.GetEnumerator();
        }

        public void Clear()
        {
            processables_.Clear();
            drawables_.Clear();
        }

        public void Add(object component)
        {
            IProcessable processable = component as IProcessable;
            if (processable != null)
            {
                processables_.Add(processable);
            }

            IDrawable drawable = component as IDrawable;
            if (drawable != null)
            {
                drawables_.Add(drawable);
            }
        }

        public void Remove(object component)
        {
            IProcessable processable = component as IProcessable;
            if (processable != null)
            {
                processables_.Remove(processable);
            }

            IDrawable drawable = component as IDrawable;
            if (drawable != null)
            {
                drawables_.Remove(drawable);
            }
        }
    }
}