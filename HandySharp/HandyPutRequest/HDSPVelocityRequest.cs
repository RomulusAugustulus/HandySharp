using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for HDSP velocity request.
    /// </summary>
    internal sealed class HDSPVelocityRequest
    {
        [JsonProperty("stopOnTarget")]
        public bool StopOnTarget { get; set; }

        [JsonProperty("immediateResponse")]
        public bool ImmediateResponse { get; set; }

        [JsonProperty("position")]
        public double Position { get; set; }

        [JsonProperty("velocity")]
        public double Velocity { get; set; }
    }
}
