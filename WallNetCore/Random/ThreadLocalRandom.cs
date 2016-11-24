using System;
using System.Threading;
using WallNetCore.Helper;

namespace WallNetCore.Random
{
    /**
        Threadsafety is gauranteed if all access is via the .Current field

        <Summary> 
            Simple threadsafe wrapper + some extension methods around the .NET Random class.
        </Summary>
    */

    public sealed class ThreadLocalRandom : System.Random, IRandom
    {
        private const int HalfwayInt = int.MaxValue / 2;

        private static readonly ThreadLocal<ThreadLocalRandom> Random =
            new ThreadLocal<ThreadLocalRandom>(() => new ThreadLocalRandom());

        public static ThreadLocalRandom Current => Random.Value;

        private ThreadLocalRandom() : base(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)) {}

        public bool NextBool()
        {
            return Next() < HalfwayInt;
        }

        public double NextDouble(double max)
        {
            Validate.Validate.Hard.IsPositive(max, () => $"Expected {max} to be positive.");
            return WallMath.BoundedDouble(max, NextDouble() * max);
        }

        public double NextDouble(double min, double max)
        {
            Validate.Validate.Hard.IsFalse(max <= min,
                () => $"{nameof(min)} ({min}) must be greater than {nameof(max)} ({max}). Arguments reversed?");
            return InternalNextDouble(min, max);
        }

        public float NextFloat()
        {
            byte[] byteBuffer = {0, 0, 0, 0};
            NextBytes(byteBuffer);
            return (BitConverter.ToUInt32(byteBuffer, 0) >> 8) * 5.960465E-008F;
        }

        public float NextFloat(float max)
        {
            Validate.Validate.Hard.IsPositive(max, () => $"Expected {max} to be positive.");
            return WallMath.BoundedFloat(max, NextFloat() * max);
        }

        public float NextFloat(float min, float max)
        {
            Validate.Validate.Hard.IsFalse(max <= min,
                () => $"{nameof(min)} ({min}) must be greater than {nameof(max)} ({max}). Arguments reversed?");
            return InternalNextFloat(min, max);
        }

        private double InternalNextDouble(double min, double max)
        {
            return WallMath.BoundedDouble(max, NextDouble() * (max - min) + min);
        }

        private float InternalNextFloat(float min, float max)
        {
            return WallMath.BoundedFloat(max, NextFloat() * (max - min) + min);
        }
    }
}