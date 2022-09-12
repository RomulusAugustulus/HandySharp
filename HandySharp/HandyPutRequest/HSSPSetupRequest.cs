using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for HSSP setup request.
    /// </summary>
    internal sealed class HSSPSetupRequest
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("sha256", NullValueHandling = NullValueHandling.Ignore)]
        public string? Sha256 { get; set; }
    }
}
