using System.Threading;

namespace WallNetCore.Random
{
    /**
        http://www.pcg-random.org/
        <summary> 
            Thread-Local random implementation of a PCG Random Number Generator. 
            Thread-Safety is gauranteed if all calls are via the .Current property
        </summary>
    */

    public static class ThreadLocalPcgRandom
    {
        public static PcgRandom Current => Random.Value;

        private static ThreadLocal<PcgRandom> Random { get; } = new ThreadLocal<PcgRandom>(() => new PcgRandom());
    }
}