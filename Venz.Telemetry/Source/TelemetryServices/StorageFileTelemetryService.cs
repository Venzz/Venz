using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Venz.Telemetry
{
    public sealed class StorageFileTelemetryService: ITelemetryService
    {
        private String FileName;
        private Lazy<Task<StorageFile>> GetStorageFileTask;

        public StorageFileTelemetryService(String fileName)
        {
            FileName = fileName;
            GetStorageFileTask = new Lazy<Task<StorageFile>>(GetStorageFileAsync);
        }

        public void Start() { }

        public void Finish() { }

        public async void LogEvent(String title) => await LogEventAsync(title);

        public async void LogEvent(String title, String parameter, String value) => await LogEventAsync(title, parameter, value);

        public async void LogException(String comment, Exception exception) => await LogExceptionAsync(comment, exception);

        public IAsyncAction LogDailyEventAsync(String title) => TaskExtensions.CompletedAction;

        public IAsyncAction LogEventAsync(String title) => LogEventAsync(title, null, null);

        public IAsyncAction LogEventAsync(String title, String parameter, String parameterValue)
        {
            if (title == null)
                return TaskExtensions.CompletedAction;

            Func<Task> asyncAction = async () =>
            {
                try
                {
                    var storageFile = await GetStorageFileTask.Value.ConfigureAwait(false);
                    var parameterString = (parameter == null) ? "" : $" >> {parameter} || {parameterValue}";
                    await FileIO.AppendTextAsync(storageFile, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {title}{parameterString}\n").AsTask().ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            };
            return Task.Run(asyncAction).AsAsyncAction();
        }

        private IAsyncAction LogExceptionAsync(String comment, Exception exception)
        {
            Func<Task> asyncAction = async () =>
            {
                try
                {
                    var storageFile = await GetStorageFileTask.Value.ConfigureAwait(false);
                    var message = (comment != null) ? $"{comment}\n\n" : "";
                    message += exception.AsDebugMessage();
                    await FileIO.AppendTextAsync(storageFile, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}\n").AsTask().ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            };
            return Task.Run(asyncAction).AsAsyncAction();
        }



        private async Task<StorageFile> GetStorageFileAsync()
        {
            var storageFile = await GetStorageFileAsync(FileName).AsTask().ConfigureAwait(false);
            await FileIO.AppendTextAsync(storageFile, "\n").AsTask().ConfigureAwait(false);
            return storageFile;
        }

        public static IAsyncOperation<StorageFile> GetStorageFileAsync(String fileName)
        {
            Func<Task<StorageFile>> asyncAction = async () =>
            {
                var fullFileName = $"{fileName} ({DateTime.Now.ToString("MMM, dd")}).log";
                var diagnosticsFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Diagnostics", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
                var logFiles = (await diagnosticsFolder.GetFilesAsync().AsTask().ConfigureAwait(false)).Where(a => a.DisplayName.StartsWith($"{fileName} (")).OrderBy(a => a.DateCreated.DateTime).ToList();
                while (logFiles.Count > 2)
                {
                    await logFiles[0].DeleteAsync().AsTask().ConfigureAwait(false);
                    logFiles.RemoveAt(0);
                }
                return await diagnosticsFolder.CreateFileAsync(fullFileName, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
            };
            return Task.Run(asyncAction).AsAsyncOperation();
        }
    }
}
