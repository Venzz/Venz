using System;
using System.Diagnostics;
using Windows.Foundation;

namespace Venz.Telemetry
{
    public sealed class DebugTelemetryService: ITelemetryService
    {
        public void Start()
        {
            Debug.WriteLine($"TELEMETRY >> Application Launched");
        }

        public void Finish()
        {
            Debug.WriteLine($"TELEMETRY >> Application Exit");
        }

        public void LogEvent(String title)
        {
            Debug.WriteLine($"TELEMETRY >> {title}");
        }

        public void LogEvent(String title, String parameter, String value)
        {
            Debug.WriteLine($"TELEMETRY >> {title} || {parameter}: {value}");
        }

        public void LogException(String comment, Exception exception)
        {
            Debug.WriteLine($"TELEMETRY >> {comment} || {exception.GetType().FullName}: {exception.Message}");
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
    }
}
