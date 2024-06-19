using Application.DTO;
using AutoMapper;
using MediatR;
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

        public GetBackupsHandler(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<List<PlayerDto>> Handle(GetBackupsRequest request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);

            if (team == null) return null;

            var backups = team.GetBackups(request.Position, request.PlayerNumber);

            return backups.Select(t => _mapper.Map<PlayerDto>(t)).ToList();
        }
    }
}
