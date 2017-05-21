using System;
using Windows.Foundation;

namespace Venz.Telemetry
{
    public sealed class EmptyTelemetryService: ITelemetryService
    {
        public void Start()
        {
        }

        public void Finish()
        {
        }

        public void LogEvent(String title)
        {
        }

        public void LogEvent(String title, String parameter, String value)
        {
        }

        public void LogException(String comment, Exception exception)
        {
        }

        public IAsyncAction LogDailyEventAsync(String title)
        {
            return TaskExtensions.CompletedAction;
        }

        public IAsyncAction LogEventAsync(String title)
        {
            return TaskExtensions.CompletedAction;
        }

        public IAsyncAction LogEventAsync(String title, String parameter, String parameterValue)
        {
            return TaskExtensions.CompletedAction;
        }
    }
}
