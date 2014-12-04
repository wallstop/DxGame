using System;
using System.Threading;

namespace DXGame.Core
{
    /**
    <summary>
        UniqueId is a thread-safe, immutable, unique identifier. 
    </summary>
    */
    public class UniqueId
    {
        private const Int64 INVALID_ID = 0;
        private static Int64 staticId;
        private static readonly UniqueId INVALID = new UniqueId(INVALID_ID);

        private readonly Int64 id_;

        public UniqueId()
            : this(GenerateId())
        {
        }

        private UniqueId(Int64 assignedId)
        {
            id_ = assignedId;
        }

        private static Int64 GenerateId()
        {
            return Interlocked.Increment(ref staticId);
        }

        public bool isValid()
        {
            return id_ != INVALID_ID;
        }

        public static UniqueId invalidId()
        {
            return INVALID;
        }
    }
}