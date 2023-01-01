using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venz.Async;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Venz.Storage
{
    public class StorageContent<T>: StorageContent where T: IStorageRecord, new()
    {
        private String Name;
        private Byte IdPrefix;
        private Boolean Initialized;
        private UInt32 LastUsedRecordId;
        private List<T> Records = new List<T>();
        private Lazy<Task> RecordsInitialization;
        private AsyncQueue ActionsQueue = new AsyncQueue();

        public StorageContent(String name, Byte idPrefix)
        {
            Name = name;
            LastUsedRecordId = (UInt32)(IdPrefix << 24);
            RecordsInitialization = new Lazy<Task>(InitializeAsync, isThreadSafe: true);
        }

        public UInt32 GetNextId()
        {
            if (!Initialized)
                throw new InvalidOperationException("Storage Content is not initialized.");
            return ++LastUsedRecordId;
        }

        private async Task InitializeAsync()
        {
            var contentStream = new InMemoryRandomAccessStream();
            var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync($"{Name}.sc", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            using (var fileStream = await contentFile.OpenReadAsync().AsTask().ConfigureAwait(false))
                await RandomAccessStream.CopyAsync(fileStream, contentStream).AsTask().ConfigureAwait(false);

            using (contentStream)
            using (var contentReader = new DataReader(contentStream))
            {
                contentStream.Seek(0);
                if (contentStream.Size < 8)
                    return;

                await contentReader.LoadAsync((UInt32)contentStream.Size).ConfigureAwait(false);
                LastUsedRecordId = contentReader.ReadUInt32();
                var recordsSize = contentReader.ReadUInt32();
                for (var i = 0; i < recordsSize; i++)
                {
                    var record = new T();
                    record.Deserialize(contentReader);
                    Records.Add(record);
                }
            }
        }

        public Task<IReadOnlyCollection<T>> GetAsync() => ActionsQueue.RunAsync<IReadOnlyCollection<T>>(async cancellation =>
        {
            await RecordsInitialization.Value.ConfigureAwait(false);
            var records = new List<T>();
            using (var stream = new InMemoryRandomAccessStream())
            using (var reader = new DataReader(stream))
            using (var writer = new DataWriter(stream))
            {
                foreach (var internalRecord in Records)
                {
                    stream.Seek(0);
                    internalRecord.Serialize(writer);
                    var record = new T();
                    stream.Seek(0);
                    record.Deserialize(reader);
                    records.Add(record);
                }
            }
            return records;
        });

        public Task AddAsync(IReadOnlyCollection<T> records) => ActionsQueue.RunAsync(async cancellation =>
        {
            if (records.Count == 0)
                return;

            await RecordsInitialization.Value.ConfigureAwait(false);
            using (var stream = new InMemoryRandomAccessStream())
            using (var reader = new DataReader(stream))
            using (var writer = new DataWriter(stream))
            {
                foreach (var record in records)
                {
                    stream.Seek(0);
                    record.Serialize(writer);

                    stream.Seek(0);
                    var newRecord = new T();
                    newRecord.Deserialize(reader);
                    Records.Add(newRecord);
                }
            }
            await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);

            /*var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync($"{Name}.sc", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            using (var contentStream = await contentFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
            using (var contentReader = new DataReader(contentStream))
            using (var contentWriter = new DataWriter(contentStream))
            {
                var recordCount = 0U;
                if (contentStream.Size >= 8)
                {
                    contentStream.Seek(4);
                    await contentReader.LoadAsync(4).ConfigureAwait(false);
                    recordCount = contentReader.ReadUInt32();
                }

                contentStream.Seek(0);
                contentWriter.Write(LastUsedRecordId);
                contentWriter.Write((UInt32)(recordCount + records.Count()));
                if (recordCount != 0)
                {
                    await contentWriter.StoreAsync().ConfigureAwait(false);
                    contentStream.Seek(contentStream.Size);
                }
                foreach (var record in records)
                {
                    record.Serialize(contentWriter);
                    Records.Add(record);
                }
                await contentWriter.StoreAsync().ConfigureAwait(false);
            }*/
        });

        public Task UpdateAsync(IReadOnlyCollection<T> records) => ActionsQueue.RunAsync(async cancellation =>
        {
            if (records.Count == 0)
                return;

            await RecordsInitialization.Value.ConfigureAwait(false);
            /*var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync($"{Name}.sc", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            using (var contentStream = await contentFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
            using (var contentWriter = new DataWriter(contentStream))
            {
                contentWriter.Write(LastUsedRecordId);
                contentWriter.Write((UInt32)Records.Count);
                foreach (var existingRecord in Records)
                {
                    var modified = false;
                    foreach (var record in records)
                    {
                        if (existingRecord.Id == record.Id)
                        {
                            record.Serialize(contentWriter);
                            modified = true;
                            break;
                        }
                    }
                    if (!modified)
                    {
                        existingRecord.Serialize(contentWriter);
                    }
                }
                await contentWriter.StoreAsync().ConfigureAwait(false);
            }
            */
            var modified = false;
            using (var stream = new InMemoryRandomAccessStream())
            using (var reader = new DataReader(stream))
            using (var writer = new DataWriter(stream))
            {
                foreach (var record in records)
                {
                    var existingRecordIndex = Records.FindIndex(a => a.Id == record.Id);
                    if (existingRecordIndex == -1)
                        continue;

                    modified = true;
                    stream.Seek(0);
                    record.Serialize(writer);

                    stream.Seek(0);
                    var updatedRecord = new T();
                    updatedRecord.Deserialize(reader);
                    Records[existingRecordIndex] = updatedRecord;
                }
            }
            if (modified)
            {
                await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);
            }
        });



        private Task TransactionFinishTask = Task.CompletedTask;
        //типа методы из общего класса

        private List<StorageTransaction> Transactions = new List<StorageTransaction>();


        // transaction id to commit??

        public Task<StorageTransaction> StartTransactionAsync() => ActionsQueue.RunAsync(cancellation =>
        {
            lock (this)
            {
                var transaction = new StorageTransaction();
                Transactions.Add(transaction);
                ActionsQueue.Add(a => transaction.Wait());
                return transaction;
            }
        });

        public async Task CommitTransactionAsync(StorageTransaction transaction)
        {
            await transaction.Queue.RunAsync(cancellation => transaction.Finish()).ConfigureAwait(false);
            transaction.Queue.Dispose();
            lock (this)
            {
                Transactions.Remove(transaction);
            }
        }

        public async Task DeleteAsync(IEnumerable<UInt32> records, AsyncQueue transactionQueue = null)
        {
            if (records.Count() == 0)
                return;

            var executionQueue = (transactionQueue != null) ? transactionQueue : ActionsQueue;
            await executionQueue.RunAsync(async cancellation =>
            {
                await RecordsInitialization.Value.ConfigureAwait(false);
                var recordIds = new HashSet<UInt32>(records);
                if (Records.RemoveAll(a => recordIds.Contains(a.Id)) > 0)
                    await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);


                /*var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync($"{Name}.sc", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
                using (var contentStream = await contentFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                using (var contentReader = new DataReader(contentStream))
                using (var contentWriter = new DataWriter(contentStream))
                {
                    if (contentStream.Size < 8)
                        return;

                    await contentReader.LoadAsync((UInt32)contentStream.Size).ConfigureAwait(false);
                    var lastUsedRecordId = contentReader.ReadUInt32();
                    var recordsSize = contentReader.ReadUInt32();
                    var existingRecords = new List<T>();
                    for (var i = 0; i < recordsSize; i++)
                    {
                        var existingRecord = new T();
                        existingRecord.Deserialize(contentReader);
                        if (records.Count(a => a == existingRecord.Id) == 0)
                            existingRecords.Add(existingRecord);
                    }

                    contentStream.Seek(0);
                    contentWriter.Write(LastUsedRecordId);
                    contentWriter.Write((UInt32)existingRecords.Count);
                    foreach (var record in existingRecords)
                        record.Serialize(contentWriter);

                    contentStream.Size = contentStream.Position;
                    await contentWriter.StoreAsync().ConfigureAwait(false);
                }*/
            });
        }

        private static async Task RecreateAsync(String fileName, UInt32 lastUsedRecordId, IReadOnlyCollection<T> records)
        {
            var contentStream = new InMemoryRandomAccessStream();
            using (var contentReader = new DataReader(contentStream))
            using (var contentWriter = new DataWriter(contentStream))
            {
                contentWriter.Write(lastUsedRecordId);
                contentWriter.Write(records.Count());
                foreach (var record in records)
                    record.Serialize(contentWriter);
                await contentWriter.StoreAsync().ConfigureAwait(false);
            }

            var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            using (var fileStream = await contentFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                await RandomAccessStream.CopyAndCloseAsync(contentStream, fileStream).AsTask().ConfigureAwait(false);
        }


        internal override async Task<IReadOnlyCollection<T1>> GetAsync<T1>()
        {
            await RecordsInitialization.Value.ConfigureAwait(false);
            var records = new List<T1>();
            using (var stream = new InMemoryRandomAccessStream())
            using (var reader = new DataReader(stream))
            using (var writer = new DataWriter(stream))
            {
                foreach (var internalRecord in Records)
                {
                    stream.Seek(0);
                    internalRecord.Serialize(writer);
                    var record = new T1();
                    stream.Seek(0);
                    record.Deserialize(reader);
                    records.Add(record);
                }
            }
            return records;
        }

        internal override Task AddAsync<T1>(IReadOnlyCollection<T1> items)
        {
            throw new NotImplementedException();
        }

        internal override Task UpdateAsync<T1>(IReadOnlyCollection<T1> items)
        {
            throw new NotImplementedException();
        }

        internal override Task DeleteAsync<T1>(IEnumerable<uint> items)
        {
            throw new NotImplementedException();
        }
    }
}
