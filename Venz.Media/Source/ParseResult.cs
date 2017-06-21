using System;

namespace Venz.Media
{
    public class ParseResult<T>
    {
        public ParseResultStatus Status { get; private set; }
        public T Content { get; private set; }
        public Exception Exception { get; private set; }

        private ParseResult() { }

        public static ParseResult<T> Create(T content) => new ParseResult<T>() { Status = ParseResultStatus.Success, Content = content };

        public static ParseResult<T> Create(Exception exception) => new ParseResult<T>() { Status = ParseResultStatus.Exception, Exception = exception };

        public static ParseResult<T> CreateUnknownFormat() => new ParseResult<T>() { Status = ParseResultStatus.UnknownFormat };
    }
}
