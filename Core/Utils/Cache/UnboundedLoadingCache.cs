using System;
using System.Runtime.Serialization;

namespace DXGame.Core.Utils.Cache
{
    /**
        <summary>
            Threadsafe unbounded loading cache. 
            Useful if you want to store a lot of stuff in a dictionary-style format, but threadsafe and automatically loading!
        </summary>
    */
    [Serializable]
    [DataContract]
    public class UnboundedLoadingCache<U, T> : AbstractCache<U, T>
    {
        private Func<U, T> Producer
        {
            get;
        }

        public UnboundedLoadingCache(Func<U, T> producer)
        {
            Validate.IsNotNull(producer, StringUtils.GetFormattedNullOrDefaultMessage(this, "producer"));
            Producer = producer;
        }

        public override T Get(U key)
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
                T value = Producer(key);
                cache_[key] = value;
                return value;
            }
        }
    }
}
