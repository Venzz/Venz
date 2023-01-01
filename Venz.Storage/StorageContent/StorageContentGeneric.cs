using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venz.Async;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Venz.Storage
{
    public class StorageContent<TRecord>: StorageContent where TRecord: IStorageRecord, new()
    {
        private String Name;
        private UInt32 LastUsedRecordId;
        private List<TRecord> Records = new List<TRecord>();
        private Lazy<Task> RecordsInitialization;
        private AsyncQueue ActionsQueue = new AsyncQueue();

        public StorageContent(String name, Byte idPrefix)
        {
            Name = name;
            LastUsedRecordId = (UInt32)(idPrefix << 24);
            RecordsInitialization = new Lazy<Task>(InitializeAsync, isThreadSafe: true);
        }

        private async Task InitializeAsync()
        {
            var contentStream = new InMemoryRandomAccessStream();
            var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(GetFileName(), CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
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
                    var record = new TRecord();
                    record.Deserialize(contentReader);
                    Records.Add(record);
                }
            }
        }

        internal override String GetFileName() => $"{Name}.sc";

        internal override async Task<UInt32> GetNextIdAsync()
        {
            await RecordsInitialization.Value.ConfigureAwait(false);
            return ++LastUsedRecordId;
        }

        internal override async Task<IReadOnlyCollection<T>> GetAsync<T>()
        {
            await RecordsInitialization.Value.ConfigureAwait(false);
            var records = new List<T>();
            using (var stream = new InMemoryRandomAccessStream())
            using (var writer = new DataWriter(stream))
            {
                foreach (var internalRecord in Records)
                {
                    stream.Seek(0);
                    internalRecord.Serialize(writer);
                    await writer.StoreAsync().ConfigureAwait(false);
                    var record = new T();
                    stream.Seek(0);
                    var reader = new DataReader(stream);
                    await reader.LoadAsync((UInt32)stream.Size).ConfigureAwait(false);
                    record.Deserialize(reader);
                    records.Add(record);
                }
            }
            return records;
        }

        internal override async Task AddAsync<T>(IReadOnlyCollection<T> records)
        {
            if (records.Count == 0)
                return;

            await RecordsInitialization.Value.ConfigureAwait(false);
            using (var stream = new InMemoryRandomAccessStream())
            using (var writer = new DataWriter(stream))
            {
                foreach (var record in records)
                {
                    stream.Seek(0);
                    record.Serialize(writer);
                    await writer.StoreAsync().ConfigureAwait(false);

                    stream.Seek(0);
                    var reader = new DataReader(stream);
                    await reader.LoadAsync((UInt32)stream.Size).ConfigureAwait(false);
                    var newRecord = new TRecord();
                    newRecord.Deserialize(reader);
                    Records.Add(newRecord);
                }
            }
            await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);
        }

        internal override async Task UpdateAsync<T>(IReadOnlyCollection<T> records)
        {
            if (records.Count == 0)
                return;

            await RecordsInitialization.Value.ConfigureAwait(false);
            var modified = false;
            using (var stream = new InMemoryRandomAccessStream())
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
                    await writer.StoreAsync().ConfigureAwait(false);

                    stream.Seek(0);
                    var reader = new DataReader(stream);
                    await reader.LoadAsync((UInt32)stream.Size).ConfigureAwait(false);
                    var updatedRecord = new TRecord();
                    updatedRecord.Deserialize(reader);
                    Records[existingRecordIndex] = updatedRecord;
                }
            }
            if (modified)
            {
                await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);
            }
        }

        internal override async Task SaveAsync<T>(IReadOnlyCollection<T> records)
        {
            if (records.Count == 0)
                return;

            await RecordsInitialization.Value.ConfigureAwait(false);
            using (var stream = new InMemoryRandomAccessStream())
            using (var writer = new DataWriter(stream))
            {
                foreach (var record in records)
                {
                    stream.Seek(0);
                    record.Serialize(writer);
                    await writer.StoreAsync().ConfigureAwait(false);

                    stream.Seek(0);
                    var reader = new DataReader(stream);
                    await reader.LoadAsync((UInt32)stream.Size).ConfigureAwait(false);
                    var updatedRecord = new TRecord();
                    updatedRecord.Deserialize(reader);

                    var existingRecordIndex = Records.FindIndex(a => a.Id == record.Id);
                    if (existingRecordIndex == -1)
                        Records.Add(updatedRecord);
                    else
                        Records[existingRecordIndex] = updatedRecord;
                }
            }
            await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);
        }

        internal override async Task DeleteAsync(IEnumerable<UInt32> records)
        {
            if (records.Count() == 0)
                return;

            await RecordsInitialization.Value.ConfigureAwait(false);
            var recordIds = new HashSet<UInt32>(records);
            if (Records.RemoveAll(a => recordIds.Contains(a.Id)) > 0)
                await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);
        }

        internal override async Task ResetAsync<T>(IReadOnlyCollection<T> records)
        {
            await RecordsInitialization.Value.ConfigureAwait(false);
            Records.Clear();
            using (var stream = new InMemoryRandomAccessStream())
            using (var writer = new DataWriter(stream))
            {
                foreach (var record in records)
                {
                    stream.Seek(0);
                    record.Serialize(writer);
                    await writer.StoreAsync().ConfigureAwait(false);

                    stream.Seek(0);
                    var reader = new DataReader(stream);
                    await reader.LoadAsync((UInt32)stream.Size).ConfigureAwait(false);
                    var newRecord = new TRecord();
                    newRecord.Deserialize(reader);
                    Records.Add(newRecord);
                }
            }
            await RecreateAsync($"{Name}.sc", LastUsedRecordId, Records).ConfigureAwait(false);
        }

        internal override async Task<IRandomAccessStream> GetDataAsStreamAsync()
        {
            var dataStream = new InMemoryRandomAccessStream();
            var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(GetFileName(), CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            using (var fileStream = await contentFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
                await RandomAccessStream.CopyAsync(fileStream, dataStream).AsTask().ConfigureAwait(false);
            return dataStream;
        }



        private static async Task RecreateAsync(String fileName, UInt32 lastUsedRecordId, IReadOnlyCollection<TRecord> records)
        {
            var contentStream = new InMemoryRandomAccessStream();
            var contentWriter = new DataWriter(contentStream);
            contentWriter.Write(lastUsedRecordId);
            contentWriter.Write(records.Count);
            foreach (var record in records)
                record.Serialize(contentWriter);
            await contentWriter.StoreAsync().ConfigureAwait(false);

            contentStream.Seek(0);
            var contentFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            using (var fileStream = await contentFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false))
            using (contentStream)
                await RandomAccessStream.CopyAsync(contentStream, fileStream).AsTask().ConfigureAwait(false);
        }
    }
}
