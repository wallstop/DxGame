using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Threading;

namespace DxCore.Core.Utils
{
    /**
        <summary>
            Essentialy a UniqueId-clone, except instances are used to create new instances
        </summary>
    */

    [DataContract]
    [Serializable]
    public sealed class IdBasedCounter : IComparable, IEquatable<IdBasedCounter>
    {
        private static int ID_TRACKER;

        private static readonly ConcurrentDictionary<int, IdBasedCounter> COUNTERS_BY_ID =
            new ConcurrentDictionary<int, IdBasedCounter>();

        private readonly long count_;

        public static int NewId => Interlocked.Increment(ref ID_TRACKER);

        public IdBasedCounter Next => new IdBasedCounter(count_ + 1);

        private IdBasedCounter() : this(0) {}

        private IdBasedCounter(long count)
        {
            count_ = count;
        }

        public int CompareTo(object other)
        {
            IdBasedCounter idBasedCounter = other as IdBasedCounter;
            if(ReferenceEquals(idBasedCounter, null))
            {
                return -1;
            }
            return -count_.CompareTo(idBasedCounter.count_);
        }

        public bool Equals(IdBasedCounter other)
        {
            return count_ == other?.count_;
        }

        public static IdBasedCounter NewCount(int id)
        {
            return COUNTERS_BY_ID.AddOrUpdate(id, new IdBasedCounter(),
                (key, existingIdBasedCount) => existingIdBasedCount.Next);
        }

        public override bool Equals(object other)
        {
            IdBasedCounter idBasedCounter = other as IdBasedCounter;
            return Equals(idBasedCounter);
        }

        public override int GetHashCode()
        {
            return count_.GetHashCode();
        }

        public override string ToString()
        {
            return count_.ToString();
        }
    }
}