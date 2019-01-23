using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Venz.Telemetry
{
    public sealed class StorageFileTelemetryService: ITelemetryService
    {
        private String FileName;
        private Lazy<Task<IRandomAccessStream>> GetLogFileStreamTask;

        public StorageFileTelemetryService(String fileName)
        {
            FileName = fileName;
            GetLogFileStreamTask = new Lazy<Task<IRandomAccessStream>>(GetLogFileStreamAsync);
        }

        public void Start() { }

        public void Finish() { }

        public async void LogEvent(String title) => await LogEventAsync(title);

        public async void LogEvent(String title, String parameter, String value) => await LogEventAsync(title, parameter, value);

        public async void LogException(String comment, Exception exception) => await LogExceptionInternalAsync(comment, exception);

        public IAsyncAction LogDailyEventAsync(String title) => TaskExtensions.CompletedAction;

        public IAsyncAction LogEventAsync(String title) => LogEventAsync(title, null, null);

        public IAsyncAction LogEventAsync(String title, String parameter, String parameterValue) => LogEventInternalAsync(title, parameter, parameterValue).AsAsyncAction();

        private async Task LogEventInternalAsync(String title, String parameter, String parameterValue)
        {
            try
            {
                if (title == null)
                    return;

                var logFileStream = await GetLogFileStreamTask.Value.ConfigureAwait(false);
                var parameterString = (parameter == null) ? "" : $" >> {parameter} || {parameterValue}";
                await AppendTextAsync(logFileStream, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {title}{parameterString}\n").ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }

        private async Task LogExceptionInternalAsync(String comment, Exception exception)
        {
            try
            {
                var logFileStream = await GetLogFileStreamTask.Value.ConfigureAwait(false);
                var message = (comment != null) ? $"{comment}\n\n" : "";
                message += exception.AsDebugMessage();
                await AppendTextAsync(logFileStream, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {message}\n").ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }



        private async Task<IRandomAccessStream> GetLogFileStreamAsync()
        {
            var logFile = await GetStorageFileAsync(FileName).AsTask().ConfigureAwait(false);
            var logStream = await logFile.OpenAsync(FileAccessMode.ReadWrite).AsTask().ConfigureAwait(false);
            logStream.Seek(logStream.Size);
            using (var dataWriter = new DataWriter(logStream))
            {
                dataWriter.WriteString("\n");
                await dataWriter.StoreAsync().AsTask().ConfigureAwait(false);
                dataWriter.DetachStream();
            }
            return logStream;
        }

        private static async Task AppendTextAsync(IRandomAccessStream logFileStream, String text)
        {
            using (var dataWriter = new DataWriter(logFileStream))
            {
                dataWriter.WriteString(text);
                await dataWriter.StoreAsync().AsTask().ConfigureAwait(false);
                dataWriter.DetachStream();
            }
        }

        public static IAsyncOperation<StorageFile> GetStorageFileAsync(String fileName)
        {
            Func<Task<StorageFile>> asyncAction = async () =>
            {
                var fullFileName = $"{fileName} ({DateTime.Now.ToString("MMM, dd")}).log";
                var diagnosticsFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Diagnostics", CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);
                var files = await diagnosticsFolder.GetFilesAsync().AsTask().ConfigureAwait(false);
                var logFiles = files.Where(a => a.DisplayName.StartsWith($"{fileName} (")).OrderBy(a => a.DateCreated.DateTime).ToList();
                while (logFiles.Count > 10)
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
