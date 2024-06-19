using Application.DTO;
using AutoMapper;
using MediatR;
using Persistence.IRepository;

namespace Application.Command
{
    public class RemovePlayerFromDepthChartRequest : IRequest<PlayerDto>
    {
        public string TeamId { get; set; }
        public string Position { get; set; }
        public int PlayerNumber { get; set; }
    }

    public class RemovePlayerFromDepthChartHandler : IRequestHandler<RemovePlayerFromDepthChartRequest, PlayerDto>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public RemovePlayerFromDepthChartHandler(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<PlayerDto> Handle(RemovePlayerFromDepthChartRequest request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);
            if (team == null) { return null; }

            var entry = team.DepthChartEntries.FirstOrDefault(e => e.Player.Number == request.PlayerNumber && e.Position == request.Position);

            if (entry == null) return null;

            team.RemovePlayerFromDepthChart(request.Position, request.PlayerNumber);
            await _teamRepository.UpdateAsync(team);

            return _mapper.Map<PlayerDto>(entry.Player);
        }
    }
}
