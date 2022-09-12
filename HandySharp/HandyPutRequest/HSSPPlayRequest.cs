using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for HSSP play request.
    /// </summary>
    internal sealed class HSSPPlayRequest
    {
        [JsonProperty("estimatedServerTime")]
        public long EstimatedServerTime { get; set; }

        [JsonProperty("startTime")]
        public long StartTime { get; set; }
    }
}
