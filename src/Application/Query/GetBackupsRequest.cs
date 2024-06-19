using Application.Command;
using Application.DTO;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence.IRepository;

namespace Application.Query
{
    public class GetBackupsRequest : IRequest<List<PlayerDto>>
    {
        public string TeamId { get; set; }
        public string Position { get; set; }
        public int PlayerNumber { get; set; }
    }
    public class GetBackupsHandler : IRequestHandler<GetBackupsRequest, List<PlayerDto>>
    {
        private readonly ITeamRepository _teamRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<GetBackupsHandler> _logger;

        public GetBackupsHandler(ITeamRepository teamRepository, IMapper mapper, ILogger<GetBackupsHandler> logger)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<PlayerDto>> Handle(GetBackupsRequest request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);

            if (team == null)
            {
                _logger.LogError($"Team does not exist for Id: {request.TeamId}.");
                return null;
            }

            var backups = team.GetBackups(request.Position, request.PlayerNumber);
            _logger.LogInformation("Backups found successfully.");

            return backups.Select(t => _mapper.Map<PlayerDto>(t)).ToList();
        }
    }
}
