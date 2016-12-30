using System;
using WallNetCore.Validate;

namespace DxCore.Core.Utils
{
    public static class GenericExtensions
    {
        public static T CheckedCast<T>(object typedObject, Func<string> mesageProducer) where T : class
        {
            var casted = typedObject as T;
            Validate.Hard.IsNotNull(casted, mesageProducer);
            return casted;
        }

        public static T CheckedCast<T>(object typedObject) where T : class
        {
            return CheckedCast<T>(typedObject, () => $"Could not cast {typedObject} to {typeof(T)}");
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