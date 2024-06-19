using MediatR;
using Application.DTO;
using Domain.Models;
using Persistence.IRepository;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Command
{
    public class AddPlayerToDepthChartRequest : IRequest<(bool, string)>
    {
        public DepthChartEntryDto DepthChartEntry { get; set; }
    }

    public class AddPlayerToDepthChartHandler : IRequestHandler<AddPlayerToDepthChartRequest, (bool, string)>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddPlayerToDepthChartHandler> _logger;
        public AddPlayerToDepthChartHandler(ITeamRepository teamRepository, IMapper mapper, ILogger<AddPlayerToDepthChartHandler> logger)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(bool, string)> Handle(AddPlayerToDepthChartRequest request, CancellationToken cancellationToken)
        {

            var team = await _teamRepository.GetByIdAsync(request.DepthChartEntry.TeamId);
            if (team == null)
            {
                _logger.LogError($"Team does not exist for Id: {request.DepthChartEntry.TeamId}.");
                return (false, "Team not found");
            }

            var player = _mapper.Map<Player>(request.DepthChartEntry.Player);
            team.AddDepthChartEntry(request.DepthChartEntry.Position, player, request.DepthChartEntry.PositionDepth ?? -1);

            await _teamRepository.UpdateAsync(team);

            _logger.LogInformation("Depth chart entry added successfully.");
            return (true, "");
        }
    }
}
