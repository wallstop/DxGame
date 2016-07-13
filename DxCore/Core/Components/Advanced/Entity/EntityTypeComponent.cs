using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Components.Advanced.Entity
{
    [Serializable]
    [DataContract]
    public class EntityTypeComponent : Component
    {
        [DataMember]
        public EntityType EntityType { get; private set; }

        public EntityTypeComponent(EntityType entityType)
        {
            Validate.Hard.IsNotNull(entityType, this.GetFormattedNullOrDefaultMessage(entityType));
            EntityType = entityType;
        }
    }
}