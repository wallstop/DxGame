using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;
using log4net.Repository.Hierarchy;

namespace DXGame.Core.Utils
{
    public static class GenericUtils
    {
        public static bool IsNullOrDefault<T>(T instance)
        {
            return EqualityComparer<T>.Default.Equals(instance, default(T));
        }

        public static void CheckNullOrDefault<T>(T instance, string message = "")
        {
            Debug.Assert(!IsNullOrDefault(instance), message);
        }

        public static void CheckNull<T>(T instance, string message = "")
        {
            Debug.Assert(null != instance, message);
        }

        /*
            Like soft fail, but throws ArgumentException. This allows for 
            both Debug and Release code to fail
        */
        public static void HardFail(ILog log, string message)
        {
            SoftFail(log, message);
            throw new ArgumentException(message);
        }

        /*
            Utilizes Debug.Assert(false, message) in order to only fail in Debug.
            Code flow will continue in Release.
        */
        public static void SoftFail(ILog log, string message)
        {
            log.Error(message);
            Debug.Assert(false, message);
        }

        public static T CheckedCast<T>(object typedObject, ILog log, string message) where T : class
        {
            var casted = typedObject as T;
            if (casted == null)
            {
                HardFail(log, message);
            }
            return casted;
        }
    }
}
