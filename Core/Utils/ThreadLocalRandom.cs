using System;
using System.Threading;
using static System.Int32;

namespace DXGame.Core.Utils
{
    /**
        Threadsafety is gauranteed if all access is via the .Current field
        <Summary> Simple threadsafe wrapper + some extension methods around the .NET Random class.</Summary>
    */

    public sealed class ThreadLocalRandom : Random
    {
        private const int HalfwayInt = MaxValue / 2;

        private static readonly ThreadLocal<ThreadLocalRandom> random_ =
            new ThreadLocal<ThreadLocalRandom>(() => new ThreadLocalRandom());

        public static ThreadLocalRandom Current => random_.Value;

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

        public bool NextBool()
        {
            return Next() < HalfwayInt;
        }
    }
}