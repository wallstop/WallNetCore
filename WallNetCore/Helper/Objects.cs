using System;
using System.Linq;

namespace WallNetCore.Helper
{
    public static class Objects
    {
        public static bool Equals<T, U>(T first, U second)
        {
            if(ReferenceEquals(first, second))
            {
                return true;
            }
            return first?.Equals(second) ?? false;
        }

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
                return args.Aggregate((int) 2166136261, (current, arg) => (current * 16777619) ^ arg?.GetHashCode() ?? 0);
            }
        }
    }
}