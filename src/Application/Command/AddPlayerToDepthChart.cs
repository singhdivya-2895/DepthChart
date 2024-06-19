using MediatR;
using Application.DTO;
using Domain.Models;
using Persistence.IRepository;
using AutoMapper;
using Persistence.Repository;
using System.Numerics;

namespace Application.Command
{
    public class AddPlayerToDepthChartRequest : IRequest<Unit>
    {
        public DepthChartEntryDto DepthChartEntry { get; set; }
    }

    public class AddPlayerToDepthChartHandler : IRequestHandler<AddPlayerToDepthChartRequest, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public AddPlayerToDepthChartHandler(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(AddPlayerToDepthChartRequest request, CancellationToken cancellationToken)
        {

            var team = await _teamRepository.GetByIdAsync(request.DepthChartEntry.TeamId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }

            var player = _mapper.Map<Player>(request.DepthChartEntry.Player);
            team.AddDepthChartEntry(request.DepthChartEntry.Position, player, request.DepthChartEntry.PositionDepth ?? -1);

            await _teamRepository.UpdateAsync(team);
            return Unit.Value;
        }
    }
}
