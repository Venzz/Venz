using Windows.ApplicationModel.Background;

namespace Venz.Background
{
    public sealed class BackgroundTask: IBackgroundTaskCancellation
    {
        private BackgroundTaskDeferral Deferral;
        private IBackgroundApplication BackgroundApplication;

        public async void Run(IBackgroundTaskInstance taskInstance, IBackgroundApplication backgroundApplication)
        {
            Deferral = taskInstance.GetDeferral();
            BackgroundApplication = backgroundApplication;
            taskInstance.Canceled += OnTaskCanceled;
            await BackgroundApplication.StartAsync(this, taskInstance.TriggerDetails);
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            BackgroundApplication.OnCanceled();
            Deferral.Complete();
        }

        public void Cancel() => Deferral.Complete();
    }
}