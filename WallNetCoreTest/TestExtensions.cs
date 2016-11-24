using System;

namespace WallNetCoreTest
{
    public static class TestExtensions
    {
        public static void RunMultipleTimes(this Action function)
        {
            const int numRunTimes = 10000;
            RunMultipleTimes(function, numRunTimes);
        }

        public static void RunMultipleTimes(this Action function, int numRunTimes)
        {
            for(int i = 0; i < numRunTimes; ++i)
            {
                function();
            }
        }
    }
}