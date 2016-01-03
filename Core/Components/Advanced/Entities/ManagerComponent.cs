﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;

namespace DXGame.Core.Components.Advanced.Entities
{
    /**
        <summary>
            Simple manager - style class for large "collections" of Components. 
        Holds weak-references to all "managed" entities. When this Component is disposed, it will attempt to dispose all components that it is managing.
        </summary>
    */

    [Serializable]
    [DataContract]
    public class ManagerComponent : Component
    {
        private readonly List<WeakReference<Component>> members_ = new List<WeakReference<Component>>();

        public List<Component> ManagedEntites => members_.Select(entityReference =>
        {
            /* Get out of here, Peter */
            Component peter;
            entityReference.TryGetTarget(out peter);
            return peter;
        }).Where(reference => !ReferenceEquals(reference, null)).ToList();

        public void Manage(Component entity)
        {
            members_.Add(new WeakReference<Component>(entity));
        }

        public override void Dispose()
        {
            ManagedEntites.ForEach(entity => entity?.Dispose());
            base.Dispose();
        }
    }
}