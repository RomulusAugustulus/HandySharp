using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandySharp.HandyResult
{
    /// <summary>
    /// Holds the state and the result of the mode.
    /// </summary>
    public sealed class SetModeResult
    {
        /// <summary>
        /// -1 - error
        ///  0 - success new mode
        ///  1 - success same mode
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Related state if awailable.
        /// </summary>
        public int State { get; set; }
    }
}
