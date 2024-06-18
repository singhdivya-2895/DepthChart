namespace Application.DTO
{
    public class DepthChartEntryDto
    {
        public string TeamId { get; set; }
        public string Position { get; set; }
        public int? PositionDepth { get; set; }
        public PlayerDto Player { get; set; }
    }
}
