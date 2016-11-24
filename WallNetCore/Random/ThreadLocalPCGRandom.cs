using System;
using System.Threading;
using WallNetCore.Helper;

namespace WallNetCore.Random
{
    /**
        http://www.pcg-random.org/
        <summary> 
            Thread-Local random implementation of a PCG Random Number Generator. 
            Thread-Safety is gauranteed if all calls are via the .Current property
        </summary>
    */

    public sealed class ThreadLocalPcgRandom : IRandom
    {
        private const uint HalfwayUint = uint.MaxValue / 2;
        private const uint IntMax = int.MaxValue + 1U;

        public static ThreadLocalPcgRandom Current => Random.Value;

        private ulong Increment { get; }

        private static ThreadLocal<ThreadLocalPcgRandom> Random { get; } =
            new ThreadLocal<ThreadLocalPcgRandom>(() => new ThreadLocalPcgRandom());

        private ulong State { get; set; }

        private ThreadLocalPcgRandom()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();
            State = BitConverter.ToUInt64(guidArray, 0);
            Increment = BitConverter.ToUInt64(guidArray, sizeof(ulong));
        }

        public int Next(int min, int max)
        {
            Validate.Validate.Hard.IsFalse(max <= min,
                () => $"{nameof(min)} ({min}) must be greater than {nameof(max)} ({max}). Arguments reversed?");

            uint range = unchecked((uint) (0L + max - min));
            return unchecked((int) (NextUint32(range) + min));
        }

        public int Next(int max)
        {
            Validate.Validate.Hard.IsPositive(max, () => $"Expected {max} to be positive");
            return unchecked((int) NextUint32(unchecked((uint) max)));
        }

        public int Next()
        {
            return unchecked((int) NextUint32(IntMax));
        }

        public bool NextBool()
        {
            return NextUint32() < HalfwayUint;
        }

        public float NextFloat()
        {
            return (NextUint32() >> 8) * 5.960465E-008F;
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

        public double NextDouble()
        {
            return NextLong() * 1.110223024625157E-016D;
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

        public long NextLong()
        {
            uint upper = NextUint32();
            uint lower = NextUint32();
            return unchecked((long) ((ulong) upper << 32) | lower);
        }

        private double InternalNextDouble(double min, double max)
        {
            return WallMath.BoundedDouble(max, NextDouble() * (max - min) + min);
        }

        private float InternalNextFloat(float min, float max)
        {
            return WallMath.BoundedFloat(max, NextFloat() * (max - min) + min);
        }

        private uint NextUint32(uint max)
        {
            /*
                https://github.com/libevent/libevent/blob/3807a30b03ab42f2f503f2db62b1ef5876e2be80/arc4random.c#L531

                http://cs.stackexchange.com/questions/570/generating-uniformly-distributed-random-numbers-using-a-coin
                Generates a uniform random number within the bound, avoiding modulo bias
            */
            uint threshold = unchecked((uint) ((0x100000000UL - max) % max));
            while(true)
            {
                uint randomValue = NextUint32();
                if(threshold <= randomValue)
                {
                    return randomValue % max;
                }
            }
        }

        private uint NextUint32()
        {
            ulong oldState = State;
            State = oldState * 6364136223846793005UL + Increment;
            uint xorShifted = unchecked((uint) (((oldState >> 18) ^ oldState) >> 27));
            int rot = unchecked((int) (oldState >> 59));
            return (xorShifted >> rot) | (xorShifted << (-rot & 31));
        }
    }
}