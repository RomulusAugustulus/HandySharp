namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the position and the result of the absolute position request.
    /// </summary>
    public sealed class SlidePositionAbsoluteResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Position is returned in millimeter from the base.
        /// </summary>
        public int Position { get; set; }
    }
}
