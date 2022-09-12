using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the information for a mode request.
    /// </summary>
    internal sealed class ModeRequest
    {
        [JsonProperty("mode")]
        public int Mode { get; set; }

        public ModeRequest(Modes mode)
        {
            Mode = (int)mode;
        }
    }
}
