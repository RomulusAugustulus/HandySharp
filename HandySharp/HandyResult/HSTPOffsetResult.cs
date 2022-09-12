namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the offset and result between server and device time.
    /// </summary>
    public sealed class HSTPOffsetResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        public int Offset { get; set; }
    }
}
