using Domain.Enums;

namespace Domain.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Sport Sport { get; set; }
    }
}
