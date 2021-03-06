using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Cache.Simple;
using WallNetCore.Validate;

namespace DxCore.Core
{
    [Serializable]
    [DataContract]
    public sealed class EntityType : IEquatable<EntityType>
    {
        private static readonly UnboundedLoadingSimpleCache<string, EntityType> EntityTypeSimpleCache =
            new UnboundedLoadingSimpleCache<string, EntityType>(name => new EntityType(name));

        [NonSerialized] [IgnoreDataMember] private int hash_;

        public static IReadOnlyCollection<EntityType> EntityTypes => EntityTypeSimpleCache.Elements;

        [DataMember]
        public string Name { get; }

        private EntityType(string name)
        {
            Name = name;
        }

        public bool Equals(EntityType other)
        {
            return Name == other?.Name;
        }

        public static EntityType EntityTypeFor(string name)
        {
            Validate.Hard.IsNotNullOrDefault(name,
                StringUtils.GetFormattedNullOrDefaultMessage(typeof(EntityType), "name"));
            return EntityTypeSimpleCache.Get(name);
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

        public override int GetHashCode()
        {
            if(hash_ == 0)
            {
                hash_ = Name.GetHashCode();
            }
            return hash_;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}