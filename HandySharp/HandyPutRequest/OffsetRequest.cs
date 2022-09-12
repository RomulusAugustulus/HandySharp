using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for an offset request.
    /// </summary>
    internal sealed class OffsetRequest
    {
        [JsonProperty("offset")]
        public int Offset { get; set; }
    }
}
