using Application.DTO;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AddTeamHandler> _logger;

        public AddTeamHandler(ITeamRepository teamRepository, IMapper mapper, ILogger<AddTeamHandler> logger)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TeamDto> Handle(AddTeamRequest request, CancellationToken cancellationToken)
        {
            var team = _mapper.Map<Team>(request.TeamDto);

            await _teamRepository.AddAsync(team);

            _logger.LogInformation("Team added successfully.");
            return _mapper.Map<TeamDto>(team);
        }
    }
}
