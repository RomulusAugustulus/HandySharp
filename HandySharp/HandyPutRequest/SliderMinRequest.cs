using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for a slider min request.
    /// </summary>
    internal sealed class SliderMinRequest
    {
        [JsonProperty("min")]
        public double Min
        {
            get => min;
            set
            {
                min = Math.Clamp(value, 0, 100);
            }
        }

        [JsonProperty("fixed")]
        public bool Fixed { get; set; }

        private double min;
    }
}
