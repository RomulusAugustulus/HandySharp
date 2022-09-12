using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the information for HSSP loop request.
    /// </summary>
    internal sealed class HSSPLoopRequest
    {
        [JsonProperty("activated")]
        public bool Activated { get; set; }
    }
}
