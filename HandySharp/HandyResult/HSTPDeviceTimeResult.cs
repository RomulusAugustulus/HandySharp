namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the device time calculated in HSTP mode.
    /// </summary>
    public sealed class HSTPDeviceTimeResult
    {        
        public long Time { get; set; }

        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }
    }
}
