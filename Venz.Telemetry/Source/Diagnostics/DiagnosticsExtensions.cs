using System;

namespace Venz.Telemetry
{
    internal static class DiagnosticsExtensions
    {
        public static void LogUnsupported(this DiagnosticsLevel diagnosticsLevel, String location, Type value)
        {
            diagnosticsLevel.Log($"Not supported value in {location}.", "Value", value.FullName);
        }

        public static void LogUnsupported(this DiagnosticsLevel diagnosticsLevel, String location, Object value)
        {
            diagnosticsLevel.Log($"Not supported value in {location}.", "Value", value.ToString());
        }

        public static void LogUnexpected(this DiagnosticsLevel diagnosticsLevel, String location, String param, Object value)
        {
            diagnosticsLevel.Log($"Unexpected situation in {location}.", param, value.ToString());
        }

        public static void LogUnexpected(this DiagnosticsLevel diagnosticsLevel, String location, Exception exception)
        {
            diagnosticsLevel.Log($"Unexpected exception in {location}.", exception);
        }
    }
}