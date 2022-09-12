namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the result, device time and round trip delay of the device.
    /// </summary>
    public sealed class HSTPSyncResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        public long Time { get; set; }

        public long Rtd { get; set; }
    }
}
