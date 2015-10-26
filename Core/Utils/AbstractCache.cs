﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace DXGame.Core.Utils
{
    [Serializable]
    [DataContract]
    public class AbstractCache<U, T> : ICache<U, T>
    {
        protected readonly ReaderWriterLockSlim globalLock_ = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        [DataMember]
        protected readonly Dictionary<U, T> cache_ = new Dictionary<U, T>();

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
