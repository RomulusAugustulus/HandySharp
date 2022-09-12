namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the returned settings of the device.
    /// </summary>
    public sealed class SettingsResult
    {
        public int Offset { get; set; }
        public int Velocity { get; set; }
        public double SlideMin { get; set; }
        public double SlideMax { get; set; }
    }
}
