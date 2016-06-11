using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DxCore.Core.Utils.ArrayExtensions;

namespace DxCore.Core.Utils
{
    public static class Objects
    {
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone",
            BindingFlags.NonPublic | BindingFlags.Instance);

        public static T FromWeakReference<T>(WeakReference<T> weakReference) where T : class
        {
            T empty;
            weakReference.TryGetTarget(out empty);
            return empty;
        }

        public static int HashCode(params object[] args)
        {
            unchecked
            {
                /* Borrowed from http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode, ez */
                return args.Aggregate((int) 2166136261, (current, arg) => current * 16777619 ^ arg?.GetHashCode() ?? 0);
            }
        }

        public static bool Equals<T, U>(T first, U second)
        {
            if(ReferenceEquals(first, second))
            {
                return true;
            }
            return !ReferenceEquals(first, null) && first.Equals(second);
        }

        //public static T Copy<T>(this T original)
        //{
        //    byte [] blob = Serializer<T>.BinarySerialize(original);
        //    T copy = Serializer<T>.BinaryDeserialize(blob);
        //    return copy;
        //}

        public static bool IsPrimitive(this Type type)
        {
            if(type == typeof(string))
            {
                return true;
            }
            return (type.IsValueType & type.IsPrimitive);
        }

        public static object Copy(this object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if(originalObject == null)
            {
                return null;
            }
            var typeToReflect = originalObject.GetType();
            if(IsPrimitive(typeToReflect))
            {
                return originalObject;
            }
            if(visited.ContainsKey(originalObject))
            {
                return visited[originalObject];
            }
            if(typeof(Delegate).IsAssignableFrom(typeToReflect))
            {
                return null;
            }
            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if(typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if(IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array) cloneObject;
                    clonedArray.ForEach(
                        (array, indices) =>
                            array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject,
            IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if(typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType,
                    BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject,
            Type typeToReflect,
            BindingFlags bindingFlags =
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
            Func<FieldInfo, bool> filter = null)
        {
            foreach(FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if(filter != null && filter(fieldInfo) == false)
                {
                    continue;
                }
                if(IsPrimitive(fieldInfo.FieldType))
                {
                    continue;
                }
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }

        public static T Copy<T>(this T original)
        {
            return (T) Copy((object) original);
        }

        /**
            Borrowed from http://stackoverflow.com/questions/8113570/c-set-a-member-object-value-using-reflection/8113612 
            and http://stackoverflow.com/questions/1198886/c-sharp-using-reflection-to-copy-base-class-properties

            <summary>
                Uses reflection to copy all fields from the non-null "source" object into "this".

                Usage: this.MapAllFieldsFrom($KnownGoodObjectThatYouWantFieldsCopiedFrom)
            </summary>
        */

        public static void MapAllFieldsFrom(this object destination, object source)
        {
            /* Copy over all fields */
            FieldInfo[] ps = source.GetType().GetFields();
            foreach(var item in ps)
            {
                var o = item.GetValue(source);
                var p = destination.GetType().GetField(item.Name);
                if(p != null)
                {
                    Type t = Nullable.GetUnderlyingType(p.FieldType) ?? p.FieldType;
                    object safeValue = o == null ? null : Convert.ChangeType(o, t);
                    p.SetValue(destination, safeValue);
                }
            }
            /* ... and properties */
            PropertyInfo[] sourceProperties =
                source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            PropertyInfo[] destinationProperties =
                destination.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            foreach(var property in sourceProperties)
            {
                var dest = destinationProperties.FirstOrDefault(x => x.Name == property.Name);
                if(dest != null && dest.CanWrite)
                {
                    dest.SetValue(destination, property.GetValue(source, null), null);
                }
            }
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(object obj)
        {
            if(obj == null)
            {
                return 0;
            }
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if(array.LongLength == 0)
                {
                    return;
                }
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position); while(walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            private readonly int[] maxLengths;
            public int[] Position;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for(int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for(int i = 0; i < Position.Length; ++i)
                {
                    if(Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for(int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}