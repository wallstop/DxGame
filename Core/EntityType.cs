using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DXGame.Core.Utils;
using DXGame.Core.Utils.Cache;
using ProtoBuf;

namespace DXGame.Core
{
    [Serializable]
    [DataContract]
    [ProtoContract]
    public sealed class EntityType : IEquatable<EntityType>
    {
        private static readonly UnboundedLoadingCache<string, EntityType> ENTITY_TYPE_CACHE =
            new UnboundedLoadingCache<string, EntityType>(name => new EntityType(name));

        public static IReadOnlyCollection<EntityType> EntityTypes => ENTITY_TYPE_CACHE.Elements;

        [DataMember]
        [ProtoMember(1)]
        public string Name { get; }

        private EntityType(string name)
        {
            Name = name;
        }

        [NonSerialized] [IgnoreDataMember] private int hash_;

        public static EntityType EntityTypeFor(string name)
        {
            Validate.IsNotNullOrDefault(name, StringUtils.GetFormattedNullOrDefaultMessage(typeof(EntityType), "name"));
            return ENTITY_TYPE_CACHE.Get(name);
        }

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = Name.GetHashCode();
            }
            return hash_;
        }

        public override bool Equals(object other)
        {
            var entityType = other as EntityType;
            if(!ReferenceEquals(entityType, null))
            {
                return Equals(entityType);
            }
            return false;
        }

        public bool Equals(EntityType other)
        {
            return Name == other?.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
