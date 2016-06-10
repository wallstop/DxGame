using System;
using System.Runtime.Serialization;
using DXGame.Core.Utils;

namespace DxCore.Core.Utils.Cache.Simple
{
    /**
        <summary>
            Simple threadsafe unbounded cache, for when you want to store a lot of stuff in a threadsafe manner.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class UnboundedSimpleCache<U, T> : AbstractSimpleCache<U, T>
    {
        public T PutIfAbsent(U key, T value)
        {
            /* 
                This doesn't really need to be locked for reading, as ContainsKey should be a read-only operation. 
                But, THREAD SAFETY FIRST BOYS 
            */
            using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Read))
            {
                T existing;
                if(cache_.TryGetValue(key, out existing))
                {
                    return existing;
                }
            }

            using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Write))
            {
                T existing;
                if(cache_.TryGetValue(key, out existing))
                {
                    return existing;
                }

                cache_[key] = value;
                return value;
            }
        }
    }
}
