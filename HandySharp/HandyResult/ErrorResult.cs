namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds an internal error passed from the handy to the application.
    /// </summary>
    internal sealed class ErrorResult
    {
        public Error? Error { get; set; }
    }

    /// <summary>
    /// One of the possible errors. See https://handyfeeling.com/api/handy/v2/docs/ for more information.
    /// </summary>
    public sealed class Error
    {
        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Message { get; set; }
        public bool Connected { get; set; }
    }

}
