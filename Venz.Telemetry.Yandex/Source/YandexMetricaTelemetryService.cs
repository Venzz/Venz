using System;
using System.Collections.Generic;
using Windows.Foundation;
using Yandex.Metrica;

namespace Venz.Telemetry.Yandex
{
    public sealed class YandexMetricaTelemetryService: ITelemetryService
    {
        private String ApiKey;
        private String AppVersion;
        private Boolean Initialized;
        private Object Sync = new Object();
        private IList<LoggedItem> LoggedItems = new List<LoggedItem>();



        public YandexMetricaTelemetryService(String apiKey, String appVersion)
        {
            ApiKey = apiKey;
            AppVersion = appVersion;
        }

        public void Start()
        {
            try
            {
                lock (Sync)
                {
                    YandexMetrica.Config.CustomAppVersion = new Version(AppVersion);
                    YandexMetrica.Config.CrashTracking = true;
                    YandexMetrica.Config.LocationTracking = false;
                    YandexMetrica.Activate(ApiKey);
                    Initialized = true;
                    foreach (var loggedItem in LoggedItems)
                        LogItem(loggedItem);
                }
            }
            catch (Exception)
            {
            }
        }

        public void Finish() { }

        public void LogEvent(String title) => LogItem(new LoggedEvent() { Title = title, Values = new Dictionary<String, String>() });

        public void LogEvent(String title, String parameter, String value) => LogItem(new LoggedEvent() { Title = title, Values = new Dictionary<String, String> { { parameter, value } } });

        public void LogException(String comment, Exception exception) => LogItem(new LoggedException() { Message = comment, Exception = exception });

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

        private void LogItem(LoggedItem item)
        {
            try
            {
                lock (Sync)
                {
                    if (!Initialized)
                    {
                        LoggedItems.Add(item);
                        return;
                    }
                }

                if (item is LoggedEvent)
                {
                    var loggedEvent = (LoggedEvent)item;
                    YandexMetrica.ReportEvent(loggedEvent.Title, loggedEvent.Values);
                }
                else if (item is LoggedException)
                {
                    var loggedException = (LoggedException)item;
                    YandexMetrica.ReportError(loggedException.Message, loggedException.Exception);
                }
            }
            catch (Exception)
            {
            }
        }



        private class LoggedItem { }

        private class LoggedEvent: LoggedItem
        {
            public String Title { get; set; }
            public Dictionary<String, String> Values { get; set; }
        }

        private class LoggedException: LoggedItem
        {
            public String Message { get; set; }
            public Exception Exception { get; set; }
        }
    }
}
