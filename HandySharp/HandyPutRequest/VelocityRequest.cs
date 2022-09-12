using Newtonsoft.Json;

namespace HandySharp.HandyPutRequest
{
    /// <summary>
    /// Encapsulates all the body information for a velocity request.
    /// </summary>
    internal sealed class VelocityRequest
    {
        [JsonProperty("velocity")]
        public double Velocity
        {
            get => velocity;
            set
            {
                velocity = Math.Clamp(value, 0, 100);
            }
        }

        private double velocity;
    }
}
