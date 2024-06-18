using Application.DTO;
using AutoMapper;
using Domain.Models;
using MediatR;
using Persistence.IRepository;

namespace Application.Command
{
    public class AddTeamRequest : IRequest<TeamDto>
    {
        public TeamDto teamDto { get; set; }
    }

    public class AddTeamHandler : IRequestHandler<AddTeamRequest, TeamDto>
    {
        private readonly IDepthChartCommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public AddTeamHandler(IDepthChartCommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        public async Task<TeamDto> Handle(AddTeamRequest request, CancellationToken cancellationToken)
        {
            var team = _mapper.Map<Team>(request.teamDto);

            await _commandRepository.AddTeamAsync(team);

            return _mapper.Map<TeamDto>(team);
        }
    }
}
