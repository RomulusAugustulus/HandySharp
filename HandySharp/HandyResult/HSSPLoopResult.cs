namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the loop-state and result of the HSSP mode.
    /// </summary>
    public sealed class HSSPLoopResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        ///  true - looping is set
        /// false - looping is not set
        /// </summary>
        public bool Activated { get; set; }
    }
}
