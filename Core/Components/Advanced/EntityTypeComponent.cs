using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public class EntityTypeComponent : Component
    {

        [DataMember]
        [ProtoMember(1)]
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
