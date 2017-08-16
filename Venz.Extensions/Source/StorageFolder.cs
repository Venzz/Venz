using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Search;

namespace Venz.Extensions
{
    public static class StorageFolderExtensions
    {
        public static async Task<IReadOnlyCollection<StorageFile>> GetAllFilesAsync(this StorageFolder folder)
        {
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4, 0))
                return await folder.GetFilesAsync(CommonFileQuery.OrderByName).AsTask().ConfigureAwait(false);
            else
                return await GetFilesLegacyAsync(folder).ConfigureAwait(false);
        }

        private static async Task<IReadOnlyCollection<StorageFile>> GetFilesLegacyAsync(StorageFolder storageFolder)
        {
            var files = new List<StorageFile>(await storageFolder.GetFilesAsync().AsTask().ConfigureAwait(false));
            foreach (var folder in await storageFolder.GetFoldersAsync().AsTask().ConfigureAwait(false))
                files.AddRange(await GetFilesLegacyAsync(folder).ConfigureAwait(false));
            return files;
        }
    }
}
