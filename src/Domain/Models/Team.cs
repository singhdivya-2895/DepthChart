using Domain.Enums;

namespace Domain.Models
{
    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Sport Sport { get; set; }

        private List<DepthChartEntry> _depthChartEntries = new List<DepthChartEntry>();
        public IReadOnlyCollection<DepthChartEntry> DepthChartEntries => _depthChartEntries.AsReadOnly();

        public Team() { }

        public Team(string id, string name, Sport sport)
        {
            Id = id;
            Name = name;
            Sport = sport;
        }

        public void AddDepthChartEntry(string position, Player player, int positionDepth = -1)
        {
            var entry = new DepthChartEntry(Id, position, positionDepth, player);

            if (positionDepth >= 0)
            {
                foreach (var existingEntry in _depthChartEntries.Where(e => e.Position == position && e.PositionDepth >= positionDepth))
                {
                    existingEntry.IncrementDepth();
                }
                entry.UpdateDepth(positionDepth);
            }
            else
            {
                entry.UpdateDepth(_depthChartEntries.Count(e => e.Position == position));
            }

            _depthChartEntries.Add(entry);
        }

        public void RemovePlayerFromDepthChart(string position, int playerNumber)
        {
            var entry = _depthChartEntries.FirstOrDefault(e => e.Position == position && e.Player.Number == playerNumber);
            if (entry != null)
            {
                _depthChartEntries.Remove(entry);
                foreach (var existingEntry in _depthChartEntries.Where(e => e.Position == position && e.PositionDepth > entry.PositionDepth))
                {
                    existingEntry.DecrementDepth();
                }
            }
        }

        public List<Player> GetBackups(string position, int playerNumber)
        {
            var entry = _depthChartEntries.FirstOrDefault(e => e.Position == position && e.Player.Number == playerNumber);
            if (entry == null) return new List<Player>();

            return _depthChartEntries
                .Where(e => e.Position == position && e.PositionDepth > entry.PositionDepth)
                .Select(e => e.Player)
                .ToList();
        }
    }
}
