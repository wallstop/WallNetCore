using System;

namespace WallNetCore.Helper
{
    public static class WallMath
    {
        /**
            http://grepcode.com/file/repository.grepcode.com/java/root/jdk/openjdk/8-b132/java/util/concurrent/ThreadLocalRandom.java#356
            <summary>
                BoundedDouble borrowed from Java's ThreadLocalRandom
            </summary>
        */

        public static double BoundedDouble(double max, double value)
        {
            return value < max ? value : BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(value) - 1);
        }

        public static float BoundedFloat(float max, float value)
        {
            return value < max
                ? value
                : BitConverter.ToSingle(
                    BitConverter.GetBytes(BitConverter.ToInt32(BitConverter.GetBytes(value), 0) - 1), 0);
        }

        public static int WrappedAdd(int value, int increment, int max)
        {
            return (value + increment) % max;
        }

        public static int WrappedIncrement(int value, int max)
        {
            return WrappedAdd(value, 1, max);
        }
    }
}