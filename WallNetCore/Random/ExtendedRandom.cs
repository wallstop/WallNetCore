using System;
using WallNetCore.Helper;

namespace WallNetCore.Random
{
    [Serializable]
    public class ExtendedRandom : System.Random, IRandom
    {
        private const int HalfwayInt = int.MaxValue / 2;

        public ExtendedRandom() : this(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)) {}

        public ExtendedRandom(int seed) : base(seed) {}

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