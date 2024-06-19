namespace Application.DTO
{
    /// <summary>
    /// Dto to create new Depth Chart Entry for the player
    /// </summary>
    public class DepthChartEntryDto
    {
        /// <summary>
        /// Id of the Team
        /// </summary>
        public string TeamId { get; set; }

        /// <summary>
        /// Position of the team. E.g. QB
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Depth of the player
        /// </summary>
        public int? PositionDepth { get; set; }
        public PlayerDto Player { get; set; }
    }
}
