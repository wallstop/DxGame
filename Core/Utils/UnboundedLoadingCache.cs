using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXGame.Core.Utils
{
    [Serializable]
    [DataContract]
    public class UnboundedLoadingCache<U, T>
    {
        private readonly ReaderWriterLockSlim globalLock_ = new ReaderWriterLockSlim();
        private readonly Dictionary<U, T> cache_ = new Dictionary<U, T>();

        private Func<U, T> Producer
        {
            get;
        }

        public ReadOnlyCollection<T> Elements
        {
            get
            {
                using(new CriticalRegion(globalLock_, CriticalRegion.LockType.Read))
                {
                    return new ReadOnlyCollection<T>(cache_.Values.ToList());
                }
            }
        }

        public UnboundedLoadingCache(Func<U, T> producer)
        {
            Validate.IsNotNull(producer, StringUtils.GetFormattedNullOrDefaultMessage(this, "producer"));
            Producer = producer;
        }

        public T Get(U key)
        {
            if(cache_.ContainsKey(key))
            {
                return cache_[key];
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
