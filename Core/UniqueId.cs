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
        private readonly long id_;

        public UniqueId()
            : this(GenerateId())
        {
        }

        public UniqueId(UniqueId copy)
            : this(copy.id_)
        {
        }

        private UniqueId(long assignedId)
        {
            id_ = assignedId;
        }

        public int CompareTo(object rhs)
        {
            var otherId = rhs as UniqueId;
            if (otherId != null)
            {
                if (otherId.id_ == id_)
                {
                    return 0;
                }
                return otherId.id_ > id_ ? 1 : -1;
            }
            return -1;
        }

        public bool Equals(UniqueId other)
        {
            return CompareTo(other) == 0;
        }

        public bool isValid()
        {
            return id_ != invalidId().id_;
        }

        public override bool Equals(object other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return id_.GetHashCode();
        }

        public override string ToString()
        {
            return "Id: " + id_;
        }

        private static long GenerateId()
        {
            return Interlocked.Increment(ref staticId);
        }

        public static UniqueId invalidId()
        {
            return INVALID;
        }
    }
}