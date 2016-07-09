using System;
using System.Collections.Generic;
using System.Threading;
using DxCore.Core.Utils;
using DxCore.Core.Utils.Validate;

namespace DxCore.Core.Service
{
    /**
        <summary>
            Manages singleton instances of service-like things, where service are whatever you want!
        </summary>
    */

    public sealed class ServiceProvider : IServiceProvider
    {
        public static readonly ServiceProvider Instance = new ServiceProvider();
        private HashSet<IService> ServicePool { get; } = new HashSet<IService>();

        private ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private ServiceProvider() {}

        /**
            <summary>
                Returns a registered service of the specified type if any exist, null otherwise.
            </summary>
        */

        public object GetService(Type serviceType)
        {
            object service;
            TryGet(serviceType, out service);
            return service;
        }

        /**
            <summary>
                Returns a registered service of the specified type if any exist, null otherwise.
            </summary>
        */

        public T GetService<T>() where T : class, IService
        {
            T foundService;
            if(TryGet(out foundService))
            {
                return foundService;
            }
            return null;
        }

        /**
            <summary>
                Attempts to deregister a Service of the provided type, if any exist.
                Returns true if a service was deregistered, false otherwise.
            </summary>
        */

        public bool Deregister<T>() where T : class, IService
        {
            T service;
            using(new CriticalRegion(Service<T>.Lock, CriticalRegion.LockType.Write))
            {
                service = Service<T>.Instance;
                Service<T>.Instance = null;
            }
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
            {
                return ServicePool.Remove(service);
            }
        }

        /**
            <summary>
                Attempts to deregister the specified service, if it is registered.
                Returns true if the service was deregistered, false otherwise.
            </summary>
        */

        public bool Deregister<T>(T instanceToDeregister) where T : class, IService
        {
            Validate.Hard.IsNotNull(instanceToDeregister);
            T service;
            using(new CriticalRegion(Service<T>.Lock, CriticalRegion.LockType.Write))
            {
                service = Service<T>.Instance;
                if(!ReferenceEquals(service, instanceToDeregister))
                {
                    return false;
                }
                Service<T>.Instance = null;
            }

            using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
            {
                return ServicePool.Remove(service);
            }
        }

        /**
            <summary>
                Returns all Services currently registered with ServiceProvider.
                Changes to the returned Set will not modify services owned by ServiceProvider.
            </summary>
        */
        public HashSet<IService> GetAll()
        {
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Read))
            {
                /* Serve up a copy to prevent meddling kids */
                return ServicePool.ToHashSet();
            }
        }

        /**
            <summary>
                Registers the provided, non-null service with the ServiceManager and its ServicePool.
                This will allow retrieval by type as well as inclusion in GetAll
            </summary>
        */

        public void Register<T>(T service) where T : class, IService
        {
            Validate.Hard.IsNotNull(service, () => $"Cannot register a null service of type {typeof(T)}");
            using(new CriticalRegion(Service<T>.Lock, CriticalRegion.LockType.Write))
            {
                Validate.Hard.IsNull(Service<T>.Instance);
                Service<T>.Instance = service;
            }
            using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
            {
                ServicePool.Add(service);
            }
        }

        /**
            <summary>
                Attempts to retrieve a registered service for the provided type. 
                Returns true if the service is found and the out value can be trusted, false otherwise.
            </summary>
        */

        public bool TryGet<T>(out T service) where T : class, IService
        {
            service = Service<T>.Instance;
            return !ReferenceEquals(service, null);
        }

        /**
            <summary>
                Attempts to retrieve a registered service for the provided type. 
                Returns true if the service is found and the out value can be trusted, false otherwise.
            </summary>
        */

        public bool TryGet(Type serviceType, out object service)
        {
            Validate.Hard.IsNotNull(serviceType);
            service = ServiceForType(serviceType);
            return !ReferenceEquals(service, null);
        }

        private object ServiceForType(Type serviceType)
        {
            Validate.Hard.IsNotNull(serviceType, "Cannot retrieve a service for a null type");
            Type serviceOfType = typeof(Service<>).MakeGenericType(serviceType);
            return serviceOfType.GetProperty("Instance").GetValue(null);
        }

        /* 
            Abuse the shit out of DotNet's Generics. The implementation is that
            there will be a concrete, unique class per type. This matches up nicely to
            our service model - we only ever want one service to exist at a time.
        */

        private static class Service<T> where T : class, IService
        {
            private static T instance_;

            public static T Instance
            {
                get
                {
                    using(new CriticalRegion(Lock, CriticalRegion.LockType.Read))
                    {
                        return instance_;
                    }
                }
                set
                {
                    using(new CriticalRegion(Lock, CriticalRegion.LockType.Write))
                    {
                        instance_ = value;
                    }
                }
            }

            public static ReaderWriterLockSlim Lock { get; } =
                new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }
    }
}