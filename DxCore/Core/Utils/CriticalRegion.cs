using System;
using System.ComponentModel;
using System.Threading;

namespace DxCore.Core.Utils
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

        private readonly ReaderWriterLockSlim lock_;
        private readonly LockType type_;

        public CriticalRegion(ReaderWriterLockSlim readWriteLock, LockType lockType)
        {
            Validate.Validate.Hard.IsNotNull(readWriteLock);
            Validate.Validate.Hard.IsNotNull(lockType);
            lock_ = readWriteLock;
            type_ = lockType;
            switch(lockType)
            {
                case LockType.Read:
                    lock_.EnterReadLock();
                    break;
                case LockType.Write:
                    lock_.EnterWriteLock();
                    break;
                default:
                    throw new InvalidEnumArgumentException(
                        $"Could not determine how to enclose a {typeof(CriticalRegion)} with {typeof(LockType)} {type_}");
            }
        }

        public void Dispose()
        {
            switch(type_)
            {
                case LockType.Read:
                    lock_.ExitReadLock();
                    break;
                case LockType.Write:
                    lock_.ExitWriteLock();
                    break;
                default:
                    throw new InvalidEnumArgumentException(
                        $"Could not determine how to exit a {typeof(CriticalRegion)} with {typeof(LockType)} {type_}");
            }
        }
    }
}