using System;

namespace Venz.Extensions
{
    public static class ExceptionExtensions
    {
        public static HResultCode GetHResultCode(this Exception source) => (HResultCode)source.HResult;
    }

    public enum HResultCode
    {
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/cc704587.aspx
        /// </summary>
        None = 0,

        /// <summary>
        /// Unspecified failure.
        /// </summary>
        Fail = unchecked((Int32)0x80004005),

        /// <summary>
        /// One or more arguments are not valid.
        /// </summary>
        InvalidArguments = unchecked((Int32)0x80070057),

        /// <summary>
        /// Cannot create a file when that file already exists.
        /// </summary>
        FileCreation_AlreadyExists = unchecked((Int32)0x800700B7),

        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        File_NotFound = unchecked((Int32)0x80070002),

        /// <summary>
        /// The filename, directory name, or volume label syntax is incorrect.
        /// </summary>
        File_SyntaxError = unchecked((Int32)0x8007007B),

        RpcServerUnavailable = unchecked((Int32)0x800706BA),

        RpcFailed = unchecked((Int32)0x800706BE)
    }
}
