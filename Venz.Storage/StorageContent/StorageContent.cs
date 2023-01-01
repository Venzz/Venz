using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Venz.Storage
{
    public abstract class StorageContent
    {
        internal abstract String GetFileName();
        internal abstract Task<UInt32> GetNextIdAsync();
        internal abstract Task<IReadOnlyCollection<T>> GetAsync<T>() where T: IStorageRecord, new();
        internal abstract Task AddAsync<T>(IReadOnlyCollection<T> records) where T: IStorageRecord;
        internal abstract Task UpdateAsync<T>(IReadOnlyCollection<T> records) where T: IStorageRecord;
        internal abstract Task SaveAsync<T>(IReadOnlyCollection<T> records) where T: IStorageRecord;
        internal abstract Task DeleteAsync(IEnumerable<UInt32> records);
        internal abstract Task ResetAsync<T>(IReadOnlyCollection<T> records) where T: IStorageRecord, new();
        internal abstract Task<IRandomAccessStream> GetDataAsStreamAsync();
    }
}
