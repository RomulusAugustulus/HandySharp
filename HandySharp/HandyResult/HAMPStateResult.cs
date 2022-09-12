namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the state and the result of the HAMP mode.
    /// </summary>
    public sealed class HAMPStateResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success 
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 1 - stopped
        /// 2 - moving
        /// </summary>
        public int State { get; set; }
    }
}
