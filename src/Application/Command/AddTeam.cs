using Application.DTO;
using AutoMapper;
using Domain.Models;
using MediatR;
using Persistence.IRepository;

namespace Application.Command
{
    public class AddTeamRequest : IRequest<TeamDto>
    {
        public TeamDto TeamDto { get; set; }
    }

    public class AddTeamHandler : IRequestHandler<AddTeamRequest, TeamDto>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public AddTeamHandler(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<TeamDto> Handle(AddTeamRequest request, CancellationToken cancellationToken)
        {
            var team = _mapper.Map<Team>(request.TeamDto);

            await _teamRepository.AddAsync(team);

            return _mapper.Map<TeamDto>(team);
        }
    }
}
