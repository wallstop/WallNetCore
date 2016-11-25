using System;
using System.Collections.Generic;
using System.Linq;
using WallNetCore.Random;

namespace WallNetCore.Extension
{
    public static class RandomExtensions
    {
        /**
            <summary>
                An arbitrary member of a collection
            </summary>
        */

        public static T FromCollection<T>(this IRandom random, ICollection<T> collection)
        {
            Validate.Validate.Hard.IsNotEmpty(collection, "Cannot pick a random element from an empty collection");
            return FromCollection(random.Next, collection);
        }

        public static T FromCollection<T>(this System.Random random, ICollection<T> collection)
        {
            Validate.Validate.Hard.IsNotEmpty(collection, "Cannot pick a random element from an empty collection");
            return FromCollection(random.Next, collection);
        }

        /**
            <summary>
                An arbitrary member of the enum
            </summary>
        */

        public static T FromEnum<T>(this IRandom random) where T : struct
        {
            Validate.Validate.Hard.IsTrue(typeof(T).IsEnum, "Cannot generate a random enum for a non-enum type");
            return FromEnum<T>(random.Next);
        }

        public static T FromEnum<T>(this System.Random random) where T : struct
        {
            Validate.Validate.Hard.IsTrue(typeof(T).IsEnum, "Cannot generate a random enum for a non-enum type");
            return FromEnum<T>(random.Next);
        }

        private static T FromCollection<T>(Func<int, int, int> randomInRange, ICollection<T> collection)
        {
            int randomIndex = randomInRange(0, collection.Count);
            return collection.ElementAt(randomIndex);
        }

        private static T FromEnum<T>(Func<int, int, int> randomInRange) where T : struct
        {
            T[] enumValues = (T[]) Enum.GetValues(typeof(T));
            int nextIndex = randomInRange(0, enumValues.Length);
            return enumValues[nextIndex];
        }
    }
}