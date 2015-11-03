using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class EntityTypeComponent : Component
    {

        [DataMember]
        public EntityType EntityType
        {
            get;
        }

        public EntityTypeComponent(EntityType entityType)
        {
            Validate.IsNotNull(entityType, StringUtils.GetFormattedNullOrDefaultMessage(this, entityType));
            EntityType = entityType;
        }
    }
}
