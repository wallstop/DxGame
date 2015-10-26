using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Utils
{
    /**
        <summary>
            Simple threadsafe unbounded cache, for when you want to store a lot of stuff in a threadsafe manner.
        </summary>
    */
    [Serializable]
    [DataContract]
    public class UnboundedCache<U, T> : AbstractCache<U, T>
    {
        public T PutIfAbsent(U key, T value)
        {
            /* 
                This doesn't really need to be locked for reading, as ContainsKey should be a read-only operation. 
                But, THREAD SAFETY FIRST BOYS 
            */
            using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Read))
            {
                if(cache_.ContainsKey(key))
                {
                    return cache_[key];
                }
            }
            using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Write))
            {
                if(cache_.ContainsKey(key))
                {
                    return cache_[key];
                }
                cache_[key] = value;
                return value;
            }
        }
    }
}
