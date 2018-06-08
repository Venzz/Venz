using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;

namespace Windows.Storage.Pickers
{
    public class UniversalFileSavePicker
    {
        private static UniversalFileSavePicker Instance;
        private TaskCompletionSource<StorageFile> ActivationAwaiter;

        public IDictionary<String, IList<String>> FileTypeChoices { get; } = new Dictionary<String, IList<String>>();
        public String DefaultFileExtension { get; set; }
        public String SuggestedFileName { get; set; }

        public IAsyncOperation<StorageFile> PickSaveFileAsync()
        {
            Instance = this;
            ActivationAwaiter = new TaskCompletionSource<StorageFile>();

            var awaiter = ActivationAwaiter;
            var filePicker = new FileSavePicker();
            foreach (var pair in FileTypeChoices)
                filePicker.FileTypeChoices.Add(pair);
            filePicker.DefaultFileExtension = DefaultFileExtension;
            filePicker.SuggestedFileName = SuggestedFileName;
            filePicker.PickSaveFileAndContinue();
            return awaiter.Task.AsAsyncOperation();
        }

        private void SetResult(StorageFile file)
        {
            ActivationAwaiter.SetResult(file);
            ActivationAwaiter = null;
        }

        private void Cancel()
        {
            ActivationAwaiter.SetResult(null);
            ActivationAwaiter = null;
        }

        public static void OnActivated(IActivatedEventArgs args)
        {
            try
            {
                if ((args is FileSavePickerContinuationEventArgs) && (Instance != null))
                {
                    Instance.SetResult(((FileSavePickerContinuationEventArgs)args).File);
                }
                else
                {
                    Instance?.Cancel();
                }
            }
            finally
            {
                Instance = null;
            }
        }
    }
}