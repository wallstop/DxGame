using System;
using System.Runtime.Serialization;
using System.Threading;

namespace DXGame.Core
{
    /**
    <summary>
        UniqueId is a thread-safe, immutable, unique identifier. 
    </summary>
    */

    [Serializable]
    [DataContract]
    public class UniqueId : IComparable, IEquatable<UniqueId>
    {
        private const long INVALID_ID = 0;
        private static long staticId;
        private static readonly UniqueId INVALID = new UniqueId(INVALID_ID);
        [DataMember] private readonly long id_;

        [NonSerialized] private int hashCode_;

        public UniqueId() : this(GenerateId()) {}

        public UniqueId(UniqueId copy) : this(copy.id_) {}

        private UniqueId(long assignedId)
        {
            id_ = assignedId;
        }

        public int CompareTo(object rhs)
        {
            var otherId = rhs as UniqueId;
            if(otherId != null)
            {
                /* 
                    We compare negative to allow for first-created UniqueIds to be 
                    "greater than " all UniqueIds created after 

                    ... why?
                */
                return -id_.CompareTo(otherId.id_);
            }
            return -1;
        }

        public bool Equals(UniqueId other)
        {
            return id_.Equals(other?.id_);
        }

        public bool IsValid()
        {
            return id_ != InvalidId().id_;
        }

        public override bool Equals(object other)
        {
            return CompareTo(other) == 0;
        }
 
        public override int GetHashCode()
        {
            if(hashCode_ == 0)
            {
                hashCode_ = id_.GetHashCode();
            }
            return hashCode_;
        }

        public override string ToString()
        {
            return "Id: " + id_;
        }

        private static long GenerateId()
        {
            return Interlocked.Increment(ref staticId);
        }

        public static UniqueId InvalidId()
        {
            return INVALID;
        }
    }
}