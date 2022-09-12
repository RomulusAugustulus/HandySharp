namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the velocity and result from the HAMP mode.
    /// </summary>
    public sealed class HAMPVelocityResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Indicating the percentage regarding to the maximum velocity.
        /// Range: 0 - 100.
        /// </summary>
        public double Velocity { get; set; }
    }
}
