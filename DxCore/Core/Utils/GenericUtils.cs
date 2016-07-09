using System;

namespace DxCore.Core.Utils
{
    public static class GenericUtils
    {
        public static T CheckedCast<T>(object typedObject, string message) where T : class
        {
            var casted = typedObject as T;
            Validate.Validate.Hard.IsNotNull(casted, message);
            return casted;
        }

        public static T CheckedCast<T>(object typedObject) where T : class
        {
            return CheckedCast<T>(typedObject, $"Could not cast {typedObject} to {typeof (T)}");
        }

        public static T Create<T>(Type type)
        {
            return (T) Activator.CreateInstance(type);
        }

        public static T Create<T>(string type)
        {
            return Create<T>(Type.GetType(type));
        }
    }
}