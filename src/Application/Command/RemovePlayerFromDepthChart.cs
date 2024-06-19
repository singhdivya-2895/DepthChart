using Application.DTO;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RemovePlayerFromDepthChartHandler> _logger;

        public RemovePlayerFromDepthChartHandler(ITeamRepository teamRepository, IMapper mapper, ILogger<RemovePlayerFromDepthChartHandler> logger)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PlayerDto> Handle(RemovePlayerFromDepthChartRequest request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);
            if (team == null)
            {
                _logger.LogError($"Team does not exist for Id: {request.TeamId}.");
                return null; 
            }

            var entry = team.DepthChartEntries.FirstOrDefault(e => e.Player.Number == request.PlayerNumber && e.Position == request.Position);

            if (entry == null) {
                _logger.LogError($"Player does not exist in team for number: {request.PlayerNumber}."); 
                return null; 
            }
            team.RemovePlayerFromDepthChart(request.Position, request.PlayerNumber);
            await _teamRepository.UpdateAsync(team);
            _logger.LogInformation("Player removed successfully.");

            return _mapper.Map<PlayerDto>(entry.Player);
        }
    }
}
