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
                Returns a random element from a collection
            </summary>
        */

        public static T FromCollection<T>(this IRandom random, ICollection<T> collection)
        {
            Validate.Validate.Hard.IsNotEmpty(collection, "Cannot pick a random element from an empty collection");
            int randomIndex = random.Next(0, collection.Count);
            return collection.ElementAt(randomIndex);
        }

        /**
            <summary>
                An arbitrary member of the enum
            </summary>
        */

        public static T FromEnum<T>(this IRandom random) where T : struct
        {
            Validate.Validate.Hard.IsTrue(typeof(T).IsEnum, "Cannot generate a random enum for a non-enum type");
            T[] enumValues = (T[]) Enum.GetValues(typeof(T));
            int nextIndex = random.Next(0, enumValues.Length);
            return enumValues[nextIndex];
        }
    }
}