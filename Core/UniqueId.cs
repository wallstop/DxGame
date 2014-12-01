using System.Threading;

namespace DXGame.Core
{
    public class UniqueId
    {
        private static long staticId;
        private const long INVALID_ID = 0;
        private static readonly UniqueId INVALID = new UniqueId(INVALID_ID);

        private readonly long id_;

        public UniqueId()
            : this(generateId())
        {
        }

        private UniqueId(long assignedId)
        {
            id_ = assignedId;
        }

        private static long generateId()
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