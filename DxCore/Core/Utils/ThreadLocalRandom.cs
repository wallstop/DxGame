using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DxCore.Core.Utils
{
    /**
        Threadsafety is gauranteed if all access is via the .Current field
        <Summary> Simple threadsafe wrapper + some extension methods around the .NET Random class.</Summary>
    */

    public sealed class ThreadLocalRandom : Random
    {
        private const int HalfwayInt = Int32.MaxValue / 2;

        private static readonly ThreadLocal<ThreadLocalRandom> Random =
            new ThreadLocal<ThreadLocalRandom>(() => new ThreadLocalRandom());

        public static ThreadLocalRandom Current => Random.Value;

        private ThreadLocalRandom()
            : base(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0))
        {
        }

        public float NextFloat()
        {
            byte[] byteBuffer = {0, 0, 0, 0};
            NextBytes(byteBuffer);
            return (BitConverter.ToUInt32(byteBuffer, 0) >> 8) * 5.960465E-008F;
        }

        public float NextFloat(float max)
        {
            if (max <= 0.0D)
            {
                throw new ArgumentOutOfRangeException($"Expected {max} to be positive.");
            }

            return BoundedFloat(max, NextFloat() * max);
        }

        public double NextDouble(double max)
        {
            if (max <= 0.0D)
            {
                throw new ArgumentOutOfRangeException($"Expected {max} to be positive.");
            }
            return BoundedDouble(max, NextDouble() * max);
        }

        public double NextDouble(double min, double max)
        {
            if (min >= max)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(min)} ({min}) must be greater than {nameof(max)} ({max}). Arguments reversed?");
            }

            return InternalNextDouble(min, max);
        }

        public float NextFloat(float min, float max)
        {
            if (min >= max)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(min)} ({min}) must be greater than {nameof(max)} ({max}). Arguments reversed?");
            }
            return InternalNextFloat(min, max);
        }

        private float InternalNextFloat(float min, float max)
        {
            return BoundedFloat(max, NextFloat() * (max - min) + min);
        }

        private double InternalNextDouble(double min, double max)
        {
            return BoundedDouble(max, NextDouble() * (max - min) + min);
        }

        private static float BoundedFloat(float max, float value)
        {
            return (value < max)
                ? value
                : (BitConverter.ToSingle(
                    BitConverter.GetBytes(BitConverter.ToInt32(BitConverter.GetBytes(value), 0) - 1), 0));
        }

        private static double BoundedDouble(double max, double value)
        {
            return (value < max) ? value : (BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(value) - 1));
        }

        public T FromEnum<T>() where T : struct
        {
            Validate.Validate.Hard.IsTrue(typeof(T).IsEnum, "Cannot generate a random enum for a non-enum type");
            T[] enumValues = (T[])Enum.GetValues(typeof(T));
            int nextIndex = (Array.IndexOf(enumValues, Next(0, enumValues.Length)));
            return enumValues[nextIndex];
        }

        public T FromCollection<T>(ICollection<T> collection)
        {
            Validate.Validate.Hard.IsNotEmpty(collection, "Cannot pick a random element from an empty collection");
            int randomIndex = Next(0, collection.Count);
            return collection.ElementAt(randomIndex);
        }

        public bool NextBool()
        {
            return Next() < HalfwayInt;
        }
    }
}