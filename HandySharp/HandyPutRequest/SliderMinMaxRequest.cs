using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for a slider min and max request.
    /// </summary>
    internal sealed class SliderMinMaxRequest
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

        [JsonProperty("max")]
        public double Max
        {
            get => max;
            set
            {
                max = Math.Clamp(value, 0, 100);
            }
        }

        private double min;
        private double max;
    }
}
