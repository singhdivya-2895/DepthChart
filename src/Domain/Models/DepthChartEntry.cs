namespace Domain.Models
{
    public class DepthChartEntry
    {
        public int Id { get; set; }
        public string TeamId { get; set; }
        public string Position { get; set; }
        public int PositionDepth { get; set; }
        public Player Player { get; set; }
        public DepthChartEntry() { }

        public DepthChartEntry(string teamId, string position, int positionDepth, Player player)
        {
            TeamId = teamId;
            Position = position;
            Player = player;
            UpdateDepth(positionDepth);
        }

        public void UpdateDepth(int depth)
        {
            PositionDepth = depth;
        }

        public void IncrementDepth()
        {
            PositionDepth++;
        }

        public void DecrementDepth()
        {
            PositionDepth--;
        }
    }
}
