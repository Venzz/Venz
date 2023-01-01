using System;
using System.Threading;
using Venz.Async;

namespace Venz.Storage
{
    public class StorageTransaction
    {
        private static UInt32 LastUsedRecordId = 0;

        public UInt32 Id { get; }
        public AsyncQueue Queue { get; } = new AsyncQueue();



        public StorageTransaction()
        {
            Id = GetNextId();
        }

        public void Wait()
        {
            lock (this)
                Monitor.Wait(this);
        }

        public void Finish()
        {
            lock (this)
                Monitor.Pulse(this);
        }



        private static UInt32 GetNextId()
        {
            lock (typeof(StorageTransaction))
                return ++LastUsedRecordId;
        }
    }
}
