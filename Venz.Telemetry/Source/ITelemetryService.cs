using System;
using Windows.Foundation;

namespace Venz.Telemetry
{
    public interface ITelemetryService
    {
        void Start();
        void Finish();
        void LogEvent(String title);
        void LogEvent(String title, String parameter, String value);
        void LogException(String comment, Exception exception);

        IAsyncAction LogDailyEventAsync(String title);
        IAsyncAction LogEventAsync(String title);
        IAsyncAction LogEventAsync(String title, String parameter, String parameterValue);
    }
}
