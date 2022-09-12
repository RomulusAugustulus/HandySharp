using HandySharp.HandyResult;

namespace HandySharp
{
    /// <summary>
    /// Represents an Error thrown by the API regarding the handy.
    /// </summary>
    public class HandyErrorException : Exception
    {
        /// <summary>
        /// A <see cref="HandyResult.Error"/> object containing further information.
        /// </summary>
        public Error Error { get; private set; }

        public HandyErrorException(Error error)
        {
            Error = error;
        }

        public HandyErrorException(Error error, string message)
            : base(message)
        {
            Error = error;
        }

        public HandyErrorException(Error error, string message, Exception inner)
            : base(message, inner)
        {
            Error = error;
        }
    }
}
