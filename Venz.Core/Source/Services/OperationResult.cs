using System;

namespace Venz.Services
{
    public class OperationResult
    {
        public Boolean IsSuccessful { get; private set; }
        public String Message { get; private set; }

        protected OperationResult() { }

        public static OperationResult CreateSuccessful() => new OperationResult() { IsSuccessful = true };
        public static OperationResult CreateFailed(String message) => new OperationResult() { Message = message };
        public static OperationResult<TValue> CreateSuccessful<TValue>(TValue value) => new OperationResult<TValue>(value) { IsSuccessful = true };
        public static OperationResult<TValue> CreateFailed<TValue>(String message) => new OperationResult<TValue>(default(TValue)) { Message = message };
        public static OperationResult<TValue> CreateFailed<TValue>(OperationResult result) => new OperationResult<TValue>(default(TValue)) { Message = result.Message };
    }

    public class OperationResult<TValue>: OperationResult
    {
        public TValue Value { get; }
        public OperationResult(TValue value) { Value = value; }
    }
}
