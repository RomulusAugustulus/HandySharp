namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the device information.
    /// </summary>
    public sealed class InfoResult
    {
        public string? FwVersion { get; set; }

        /// <summary>
        /// 0 - up-to-date
        /// 1 - update-required
        /// 2 - update-available
        /// </summary>
        public int FwStatus { get; set; }

        public int HwVersion { get; set; }

        public string? Model { get; set; }

        public string? Branch { get; set; }

        public string? SessionId { get; set; }
    }
}
