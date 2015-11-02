﻿using DXGame.Core.Components.Basic;
using DXGame.Core.Utils;
using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Components.Advanced
{
    [Serializable]
    [DataContract]
    public class EntityTypeComponent : Component
    {
        [NonSerialized]
        [IgnoreDataMember]
        private int hash_;

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

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = EntityType.GetHashCode();
            }
            return hash_;
        }

        public override bool Equals(object other)
        {
            var entityTypeComponent = other as EntityTypeComponent;
            if(!ReferenceEquals(entityTypeComponent, null))
            {
                return EntityType.Equals(entityTypeComponent.EntityType);
            }
            return false;
        }
    }
}