using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for a slider max request.
    /// </summary>
    internal sealed class SliderMaxRequest
    {
        [JsonProperty("max")]
        public double Max
        {
            get => max;
            set
            {
                max = Math.Clamp(value, 0, 100);
            }
        }

        [JsonProperty("fixed")]
        public bool Fixed { get; set; }

        private double max;
    }
}
