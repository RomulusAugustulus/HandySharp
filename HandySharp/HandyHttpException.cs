using System.Net;

namespace HandySharp
{
    /// <summary>
    /// Represents a http exception that is thrown by the handy API.
    /// </summary>
    public class HandyHttpException : Exception
    {
        /// <summary>
        /// The <see cref="HttpStatusCode"/> thrown by the exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// The milliseconds left until a new attempt can be made.
        /// </summary>
        public int Milliseconds { get; private set; }
                        
        public HandyHttpException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HandyHttpException(HttpStatusCode statusCode, int milliseconds)
        {
            StatusCode = statusCode;
            Milliseconds = milliseconds;
        }

        public HandyHttpException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HandyHttpException(HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
