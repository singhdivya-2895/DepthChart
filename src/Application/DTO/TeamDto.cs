using Domain.Enums;

namespace Application.DTO
{
    /// <summary>
    /// Dto to capture and return team.
    /// </summary>
    public class TeamDto
    {
        /// <summary>
        /// Id of the team
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Name of the team
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Sport for the team
        /// </summary>
        public Sport Sport { get; set; }
    }
}
