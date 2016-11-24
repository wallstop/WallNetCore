using System;
using System.ComponentModel;
using System.Threading;

namespace WallNetCore
{
    /**
         
         RAII (https://en.wikipedia.org/wiki/Resource_Acquisition_Is_Initialization) principles applied to 
         Locking so we can do a using(var readRegion = new CriticalRegion(readWriteLock, LockType.Read)) { // stuff }

         <summary> 
             Simple wrapper around ReaderWriterLocks to allow for use with using statements
         </summary>
     */

    public struct CriticalRegion : IDisposable
    {
        public enum LockType
        {
            Read,
            Write
        }

        private ReaderWriterLockSlim Lock { get; }

        private LockType Type { get; }

        public CriticalRegion(ReaderWriterLockSlim readWriteLock, LockType lockType)
        {
            Validate.Validate.Hard.IsNotNull(readWriteLock);
            Validate.Validate.Hard.IsNotNull(lockType);
            Lock = readWriteLock;
            Type = lockType;
            switch(lockType)
            {
                case LockType.Read:
                    Lock.EnterReadLock();
                    break;
                case LockType.Write:
                    Lock.EnterWriteLock();
                    break;
                default:
                    throw new InvalidEnumArgumentException(
                        $"Could not determine how to enclose a {typeof(CriticalRegion)} with {typeof(LockType)} {Type}");
            }
        }

        public void Dispose()
        {
            switch(Type)
            {
                case LockType.Read:
                    Lock.ExitReadLock();
                    break;
                case LockType.Write:
                    Lock.ExitWriteLock();
                    break;
                default:
                    throw new InvalidEnumArgumentException(
                        $"Could not determine how to exit a {typeof(CriticalRegion)} with {typeof(LockType)} {Type}");
            }
        }
    }
}