namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the result and mode of the device.
    /// </summary>
    public sealed class ModeResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// One of the following:
        /// 0 - HAMP
        /// 1 - HSSP
        /// 2 - HDSP
        /// 3 - MAINTENANCE
        /// 4 - HBSP
        /// </summary>
        public int Mode { get; set; }
    }
}
