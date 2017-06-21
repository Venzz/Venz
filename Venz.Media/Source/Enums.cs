using System;

namespace Venz.Media
{
    [Flags]
    public enum ParseOptions { Samples, Metadata }

    public enum ParseResultStatus { Success, UnknownFormat, Exception }
}
