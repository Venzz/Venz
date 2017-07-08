using System;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace Venz.Telemetry
{
    public sealed class ToastTelemetryService: ITelemetryService
    {
        private ToastNotifier Notifier = ToastNotificationManager.CreateToastNotifier();

        public void Start()
        {
            ShowToast("Application Launched", "");
        }

        public void Finish()
        {
            ShowToast("Application Exit", "");
        }

        public void LogEvent(String title)
        {
            ShowToast(title, "");
        }

        public void LogEvent(String title, String parameter, String value)
        {
            ShowToast(title, $"{parameter}: {value}");
        }

        public void LogException(String comment, Exception exception)
        {
            ShowToast(comment, $"{exception.GetType().FullName}: {exception.Message}");
        }

        public IAsyncAction LogDailyEventAsync(String title)
        {
            LogEvent(title);
            return TaskExtensions.CompletedAction;
        }

        public IAsyncAction LogEventAsync(String title)
        {
            LogEvent(title);
            return TaskExtensions.CompletedAction;
        }

        public IAsyncAction LogEventAsync(String title, String parameter, String parameterValue)
        {
            LogEvent(title, parameter, parameterValue);
            return TaskExtensions.CompletedAction;
        }



        private void ShowToast(String line1, String line2)
        {
            String normalize(String message, Int32 maxSize) => ((message.Length > maxSize) ? message.Remove(maxSize) : message).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
            var template =
                $@"<toast>
                    <visual>
                        <binding template='{ToastTemplateType.ToastText02}'>
                            <text id='1'>{normalize(line1, 50)}</text>
                            <text id='2'>{normalize(line2, 100)}</text>
                        </binding>
                    </visual>
                </toast>";

            var document = new XmlDocument();
            document.LoadXml(template);
            Notifier.Show(new ToastNotification(document));
        }
    }
}
