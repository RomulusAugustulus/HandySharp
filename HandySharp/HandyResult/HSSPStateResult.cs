namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the state and the result of the HSSP mode.
    /// </summary>
    public sealed class HSSPStateResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// 1 - need sync (Only firmware >= 3.2.x)
        /// 2 - need setup
        /// 3 - stopped
        /// 4 - playing
        /// </summary>
        public int State { get; set; }
    }
}
