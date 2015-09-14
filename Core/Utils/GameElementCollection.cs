using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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

    [Serializable]
    [DataContract]
    public class GameElementCollection : IEnumerable
        /* Should we deal with having this implement the ICollection interface? */
    {
        [DataMember]
        private SortedList<IDrawable> drawables_ = new SortedList<IDrawable>(Drawable.DefaultComparer);

        [DataMember]
        private SortedList<IProcessable> processables_ = new SortedList<IProcessable>(Processable.DefaultComparer);

        [IgnoreDataMember]
        public int Count => processables_.Count;
        [IgnoreDataMember]
        public IEnumerable<IProcessable> Processables => processables_;
        [IgnoreDataMember]
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