using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using WallNetCore;

namespace DxCore.Core.Utils.Cache.Simple
{
    [Serializable]
    [DataContract]
    public class AbstractSimpleCache<U, T> : ISimpleCache<U, T>
    {
        [DataMember] protected readonly Dictionary<U, T> cache_ = new Dictionary<U, T>();

        protected readonly ReaderWriterLockSlim globalLock_ = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /**
            <summary>
                Returns all Elements (values) that are currently stored in the cache
            </summary>
        */

        public virtual IReadOnlyCollection<T> Elements
        {
            get
            {
                using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Read))
                {
                    return new ReadOnlyCollection<T>(cache_.Values.ToList());
                }
            }
        }

        /**
            <summary>
                Returns the full mapping of Keys -> Values that are currently stored in this cache
            </summary>
        */

        public virtual IReadOnlyDictionary<U, T> KeyedElements
        {
            get
            {
                using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Read))
                {
                    return new ReadOnlyDictionary<U, T>(cache_);
                }
            }
        }

        public virtual T Get(U key)
        {
            using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Read))
            {
                return cache_[key];
            }
        }
    }
}