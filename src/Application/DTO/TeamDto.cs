using Domain.Enums;

namespace Application.DTO
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Sport Sport { get; set; }
    }
}
