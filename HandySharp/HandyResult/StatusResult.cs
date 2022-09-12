namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the current mode and the status of this mode.
    /// </summary>
    public sealed class StatusResult
    {
        /// <summary>
        /// One of the following:
        /// 0 - HAMP
        /// 1 - HSSP
        /// 2 - HDSP
        /// 3 - MAINTENANCE
        /// 4 - HBSP
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// If state is HAMP:
        /// 1 - stopped
        /// 2 - moving
        /// 
        /// If state is HSSP:
        /// 1 - need sync (Only firmware >= 3.2.x)
        /// 2 - need setup
        /// 3 - stopped
        /// 4 - playing
        /// 
        /// otherwise:
        /// 0
        /// </summary>
        public int Status { get; set; }
    }
}
