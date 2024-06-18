using MediatR;
using Application.DTO;
using Domain.Models;
using Persistence.IRepository;
using AutoMapper;

namespace Application.Command
{
    public class AddPlayerToDepthChartRequest : IRequest<Unit>
    {
        public int TeamId { get; set; }
        public string Position { get; set; }
        public PlayerDto Player { get; set; }
        public int? PositionDepth { get; set; }
    }

    public class AddPlayerToDepthChartHandler : IRequestHandler<AddPlayerToDepthChartRequest, Unit>
    {
        private readonly IDepthChartCommandRepository _commandRepository;
        private readonly IDepthChartQueryRepository _queryRepository;
        private readonly IMapper _mapper;
        public AddPlayerToDepthChartHandler(IDepthChartCommandRepository commandRepository, IDepthChartQueryRepository queryRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(AddPlayerToDepthChartRequest request, CancellationToken cancellationToken)
        {
            var entries = await _queryRepository.GetDepthChartEntriesAsync(request.TeamId);
            int depth = request.PositionDepth ?? entries.Count(e => e.Position == request.Position);

            if (request.PositionDepth.HasValue)
            {
                foreach (var entry in entries.Where(e => e.Position == request.Position && e.PositionDepth >= request.PositionDepth.Value))
                {
                    entry.PositionDepth++;
                }
            }

            var depthChartEntry = new DepthChartEntry
            {
                TeamId = request.TeamId,
                Position = request.Position,
                PositionDepth = depth,
                Player = _mapper.Map<Player>(request.Player)
            };

            await _commandRepository.AddPlayerToDepthChartAsync(depthChartEntry);
            return Unit.Value;
        }
    }
}
