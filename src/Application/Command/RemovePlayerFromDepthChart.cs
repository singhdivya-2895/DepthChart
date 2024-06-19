using Application.DTO;
using AutoMapper;
using MediatR;
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
        private readonly IDepthChartCommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public RemovePlayerFromDepthChartHandler(IDepthChartCommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        public async Task<PlayerDto> Handle(RemovePlayerFromDepthChartRequest request, CancellationToken cancellationToken)
        {
            var depthChartEntries = await _commandRepository.GetDepthChartEntriesAsync(request.TeamId, includePlayers: true, request.Position);
            var entry = depthChartEntries.FirstOrDefault(e => e.Player.Number == request.PlayerNumber);
            if (entry == null) return null;

            var backups = depthChartEntries
                            .Where(d => d.PositionDepth > entry.PositionDepth)
                            .ToList();

            backups.ForEach(c => c.PositionDepth--);

            await _commandRepository.RemovePlayerFromDepthChartAsync(entry);

            return _mapper.Map<PlayerDto>(entry.Player);
        }
    }
}
