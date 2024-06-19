using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.IRepository
{
    public interface ITeamRepository
    {
        Task<Team> GetByIdAsync(string teamId);
        Task AddAsync(Team team);
        Task UpdateAsync(Team team);
        Task RemoveAsync(Team team);
        Task<List<Team>> GetTeamsBySportAsync(Sport sport);

    }
}
