namespace HandySharp.HandyResult
{
    /// <summary>
    /// Encapsulates a result and additional information.
    /// </summary>
    /// <typeparam name="T">The type of the additional information.</typeparam>
    public sealed class Result<T>
    {
        public bool Success { get; private set; }
        public T? Message { get; private set; }

        public Result(bool success, T? message)
        {
            Success = success;
            Message = message;
        }
    }
}
