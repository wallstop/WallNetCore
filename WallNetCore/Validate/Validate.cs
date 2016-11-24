using System;
using System.Diagnostics;

namespace WallNetCore.Validate
{
    /**
        <code>
            Validate.Hard.IsNotNull(null, "Woops, guess I shouldn't have passed null"); // Throws
        </code>
    */

    public static class Validate
    {
        /** 
            <summary>
                Throws Debug Assertions if validation case fails
            </summary> 
        */

        public static Validator Assert { get; } =
            new Validator(messageProducer => Debug.Assert(false, messageProducer()));

        /** 
            <summary>
                Returns false if validation case fails
            </summary>
        */
        public static Validator Check { get; } = new Validator(_ => { });
        /** 
            <summary>
                Throws unchecked exceptions if validation case fails
            </summary> 
        */

        public static Validator Hard { get; } =
            new Validator(messageProducer => { throw new ArgumentException(messageProducer()); });
    }
}