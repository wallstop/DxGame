using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.DataStructures;
using ProtoBuf;

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
    [ProtoContract]
    public class GameElementCollection : IEnumerable
        /* Should we deal with having this implement the ICollection interface? */
    {
        [ProtoMember(1)] [DataMember] private readonly SortedList<IDrawable> drawables_;

        [ProtoMember(3)] [DataMember] private readonly SortedList<IProcessable> processables_;

        [IgnoreDataMember]
        public int Count => processables_.Count;

        [IgnoreDataMember]
        public IEnumerable<IProcessable> Processables => processables_;

        [IgnoreDataMember]
        public IEnumerable<IDrawable> Drawables => drawables_;

        public GameElementCollection()
        {
            drawables_ = new SortedList<IDrawable>(Drawable.DefaultComparer);
            processables_ = new SortedList<IProcessable>(Processable.DefaultComparer);
        }

        public GameElementCollection(GameElementCollection copy)
        {
            Validate.IsNotNull(copy, StringUtils.GetFormattedNullOrDefaultMessage(this, copy));
            drawables_ = new SortedList<IDrawable>(copy.drawables_);
            processables_ = new SortedList<IProcessable>(copy.processables_);
        }

        public IEnumerator GetEnumerator()
        {
            // TODO: Make this not suck
            var allOwnedElements = new HashSet<object>();
            foreach(var processable in processables_.Where(processable => !allOwnedElements.Contains(processable)))
            {
                allOwnedElements.Add(processable);
            }
            foreach(var drawable in drawables_.Where(drawable => !allOwnedElements.Contains(drawable)))
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
            if(processable != null)
            {
                processables_.Add(processable);
            }

            IDrawable drawable = component as IDrawable;
            if(drawable != null)
            {
                drawables_.Add(drawable);
            }
        }

        public void Remove(Predicate<object> match)
        {
            processables_.RemoveAll(match);
            drawables_.RemoveAll(match);
        }

        public void Remove(object component)
        {
            IProcessable processable = component as IProcessable;
            if(processable != null)
            {
                processables_.Remove(processable);
            }

            IDrawable drawable = component as IDrawable;
            if(drawable != null)
            {
                drawables_.Remove(drawable);
            }
        }
    }
}