using System;
using System.Runtime.Serialization;
using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;

namespace DXGame.Core.Components.Advanced
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
