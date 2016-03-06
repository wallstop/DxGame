using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Primitives;
using DXGame.Core.Utils;
using ProtoBuf;

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
    [ProtoContract]
    public class ManagerComponent : Component
    {
        [DataMember] [ProtoMember(1)] private readonly List<WeakReference<Component>> members_ =
            new List<WeakReference<Component>>();

        public List<Component> ManagedEntites
            =>
                members_.Select<WeakReference<Component>, Component>(Objects.FromWeakReference)
                    .Where(reference => !ReferenceEquals(reference, null))
                    .ToList();

        public void Manage(Component entity)
        {
            members_.Add(new WeakReference<Component>(entity));
        }

        protected override void Update(DxGameTime gameTime)
        {
            members_.RemoveAll(weakReference => ReferenceEquals(Objects.FromWeakReference(weakReference), null));
            base.Update(gameTime);
        }

        public override void Remove()
        {
            ManagedEntites.ForEach(entity => entity?.Remove());
            base.Remove();
        }
    }
}