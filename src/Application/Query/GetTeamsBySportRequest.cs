using Application.DTO;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Persistence.IRepository;

namespace Application.Query
{
    public class GetTeamsBySportRequest : IRequest<List<TeamDto>>
    {
        public Sport Sport { get; set; }
    }
    public class GetTeamsBySportHandler : IRequestHandler<GetTeamsBySportRequest, List<TeamDto>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public GetTeamsBySportHandler(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<List<TeamDto>> Handle(GetTeamsBySportRequest request, CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetTeamsBySportAsync(request.Sport);

            return teams.Select(t => _mapper.Map<TeamDto>(t)).ToList();
        }
    }
}
