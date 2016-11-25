using System.Threading;

namespace WallNetCore.Random
{
    /**
        Threadsafety is gauranteed if all access is via the .Current field

        <Summary> 
            Simple threadsafe wrapper + some extension methods around the .NET Random class.
        </Summary>
    */

    public static class ThreadLocalRandom
    {
        private static readonly ThreadLocal<ExtendedRandom> Random =
            new ThreadLocal<ExtendedRandom>(() => new ExtendedRandom());

        public static ExtendedRandom Current => Random.Value;
    }
}