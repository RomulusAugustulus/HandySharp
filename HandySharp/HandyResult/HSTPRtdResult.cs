namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the round trip delay and result during HSTP mode.
    /// </summary>
    public sealed class HSTPRtdResult
    {
        public long Rtd { get; set; }

        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

    }
}
