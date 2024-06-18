using Application.DTO;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Persistence.IRepository;

namespace Application.Command
{
    public class AddTeamRequest : IRequest<TeamDto>
    {
        public string Name { get; set; }
        public Sport Sport { get; set; }
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
            var team = new Team
            {
                Name = request.Name,
                Sport = request.Sport
            };

            await _commandRepository.AddTeamAsync(team);

            return _mapper.Map<TeamDto>(team);
        }
    }
}
