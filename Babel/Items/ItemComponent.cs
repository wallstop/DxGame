using System;
using System.Runtime.Serialization;
using DxCore.Core;
using DxCore.Core.Components.Basic;
using WallNetCore.Validate;

namespace Babel.Items
{
    /**
        TODO: Flesh this out as more Items are implemented (better ideas of what parts are common)
    */

    [DataContract]
    [Serializable]
    public abstract class ItemComponent : Component
    {
        [DataMember]
        protected int StackCount { get; set; }

        public void Attach(GameObject parent)
        {
            ++StackCount;
            InternalAttach(parent);
        }

        public void Detach(GameObject parent)
        {
            Validate.Hard.IsTrue(StackCount > 0, $"Cannot detach {this} - it has a stack count of 0!");
            // TODO: Dispose when fully detached?
            --StackCount;
            InternalDetach(parent);
        }

        protected abstract void InternalAttach(GameObject parent);
        protected abstract void InternalDetach(GameObject parent);
    }
}