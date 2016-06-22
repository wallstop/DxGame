﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DxCore.Core.DataStructures;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils
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
        [DataMember] private readonly SortedList<IDrawable> drawables_;

        [DataMember] private readonly SortedList<IProcessable> processables_;

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
            Validate.Validate.Hard.IsNotNull(copy, this.GetFormattedNullOrDefaultMessage(copy));
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