namespace Domain.Models
{
    public class DepthChartEntry
    {
        public int Id { get; set; }
        public string TeamId { get; set; }
        public string Position { get; set; }
        public int PositionDepth { get; set; }
        public Player Player { get; set; }
    }
}
