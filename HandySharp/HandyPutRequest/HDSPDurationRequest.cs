using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for a HDSP duration request.
    /// </summary>
    internal sealed class HDSPDurationRequest
    {
        [JsonProperty("stopOnTarget")]
        public bool StopOnTarget { get; set; }

        [JsonProperty("immediateResponse")]
        public bool ImmediateResponse { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("position")]
        public double Position { get; set; }
    }
}
