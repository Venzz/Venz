using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Venz.Async;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Venz.Storage
{
    public abstract class Storage
    {
        private List<StorageContent> ContentItems = new List<StorageContent>();
        private Dictionary<UInt32, StorageTransaction> Transactions = new Dictionary<UInt32, StorageTransaction>();
        private AsyncQueue ActionsQueue = new AsyncQueue();



        public void AddContent(StorageContent content)
        {
            ContentItems.Add(content);
        }

        public Task<UInt32> GetNextIdAsync<T>() where T: IStorageRecord
        {
            foreach (var storageContent in ContentItems)
                if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                    return storageContent.GetNextIdAsync();

            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }

        protected Task<IReadOnlyCollection<T>> GetAsync<T>(UInt32 transactionId = 0) where T: IStorageRecord, new()
        {
            var transaction = Transactions.GetValueOrDefault(transactionId);
            var queue = transaction?.Queue ?? ActionsQueue;
            return queue.RunAsync(cancellation =>
            {
                foreach (var storageContent in ContentItems)
                    if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                        return storageContent.GetAsync<T>();

                throw new InvalidOperationException();
            });
        }

        protected Task AddAsync<T>(IReadOnlyCollection<T> records, UInt32 transactionId = 0) where T: IStorageRecord
        {
            var transaction = Transactions.GetValueOrDefault(transactionId);
            var queue = transaction?.Queue ?? ActionsQueue;
            return queue.RunAsync(cancellation =>
            {
                foreach (var storageContent in ContentItems)
                    if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                        return storageContent.AddAsync<T>(records);

                throw new InvalidOperationException();
            });
        }

        protected Task UpdateAsync<T>(IReadOnlyCollection<T> records, UInt32 transactionId = 0) where T: IStorageRecord
        {
            var transaction = Transactions.GetValueOrDefault(transactionId);
            var queue = transaction?.Queue ?? ActionsQueue;
            return queue.RunAsync(cancellation =>
            {
                foreach (var storageContent in ContentItems)
                    if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                        return storageContent.UpdateAsync<T>(records);

                throw new InvalidOperationException();
            });
        }

        protected Task SaveAsync<T>(IReadOnlyCollection<T> records, UInt32 transactionId = 0) where T: IStorageRecord
        {
            var transaction = Transactions.GetValueOrDefault(transactionId);
            var queue = transaction?.Queue ?? ActionsQueue;
            return queue.RunAsync(cancellation =>
            {
                foreach (var storageContent in ContentItems)
                    if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                        return storageContent.SaveAsync(records);

                throw new InvalidOperationException();
            });
        }

        protected Task DeleteAsync<T>(IEnumerable<UInt32> records, UInt32 transactionId = 0) where T: IStorageRecord
        {
            var transaction = Transactions.GetValueOrDefault(transactionId);
            var queue = transaction?.Queue ?? ActionsQueue;
            return queue.RunAsync(cancellation =>
            {
                foreach (var storageContent in ContentItems)
                    if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                        return storageContent.DeleteAsync(records);

                throw new InvalidOperationException();
            });
        }

        protected Task ResetAsync<T>(IReadOnlyCollection<T> records, UInt32 transactionId = 0) where T: IStorageRecord, new()
        {
            var transaction = Transactions.GetValueOrDefault(transactionId);
            var queue = transaction?.Queue ?? ActionsQueue;
            return queue.RunAsync(cancellation =>
            {
                foreach (var storageContent in ContentItems)
                    if (storageContent.GetType().GetGenericArguments()[0] == typeof(T))
                        return storageContent.ResetAsync(records);

                throw new InvalidOperationException();
            });
        }

        public Task<UInt32> StartTransactionAsync() => ActionsQueue.RunAsync(cancellation =>
        {
            lock (this)
            {
                var transaction = new StorageTransaction();
                Transactions.Add(transaction.Id, transaction);
                ActionsQueue.Add(a => transaction.Wait());
                return transaction.Id;
            }
        });

        public async Task CommitTransactionAsync(UInt32 transactionId)
        {
            var transaction = Transactions[transactionId];
            await transaction.Queue.RunAsync(cancellation => transaction.Finish()).ConfigureAwait(false);
            transaction.Queue.Dispose();
            lock (this)
            {
                Transactions.Remove(transactionId);
            }
        }

        public Task<IRandomAccessStream> ArchiveAsync() => ActionsQueue.RunAsync<IRandomAccessStream>(async cancellation =>
        {
            var stateArchiveStream = new InMemoryRandomAccessStream();
            using (var zipArchive = new ZipArchive(stateArchiveStream.AsStream(), ZipArchiveMode.Create, true))
            {
                foreach (var storageContent in ContentItems)
                {
                    var storageContentEntry = zipArchive.CreateEntry(storageContent.GetFileName(), CompressionLevel.Fastest);
                    using (var storageContentEntryStream = storageContentEntry.Open())
                    using (var dataStream = await storageContent.GetDataAsStreamAsync().ConfigureAwait(false))
                        dataStream.AsStream().CopyTo(storageContentEntryStream);
                }
            }
            return stateArchiveStream;
        });
    }
}
