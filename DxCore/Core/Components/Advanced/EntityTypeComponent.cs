using System;
using System.Runtime.Serialization;
using DxCore.Core.Components.Basic;
using DxCore.Core.Utils;
using DXGame.Core;
using DXGame.Core.Utils;

namespace DxCore.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class EntityTypeComponent : Component
    {
        [DataMember]
        public EntityType EntityType { get; private set; }

        public EntityTypeComponent(EntityType entityType)
        {
            Validate.IsNotNull(entityType, StringUtils.GetFormattedNullOrDefaultMessage(this, entityType));
            EntityType = entityType;
        }
    }
}
