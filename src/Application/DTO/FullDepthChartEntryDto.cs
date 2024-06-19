using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    /// <summary>
    /// DTO to return the full depth chart
    /// </summary>
    public class FullDepthChartEntryDto
    {
        /// <summary>
        /// Position Depth of the player
        /// </summary>
        public int? PositionDepth { get; set; }
        public PlayerDto Player { get; set; }
    }
}
