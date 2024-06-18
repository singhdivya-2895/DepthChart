namespace Application.DTO
{
    public class DepthChartEntryDto
    {
        public int TeamId { get; set; }
        public string Position { get; set; }
        public int PositionDepth { get; set; }
        public PlayerDto Player { get; set; }
    }
}
