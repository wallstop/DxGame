using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DXGame.Core
{
    [Serializable]
    [DataContract]
    public sealed class EntityType : IEquatable<EntityType>
    {
        public bool Equals(EntityType other)
        {
            throw new NotImplementedException();
        }
    }
}
