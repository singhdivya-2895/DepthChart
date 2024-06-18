using MediatR;
using Application.DTO;
using Domain.Models;
using Persistence.IRepository;
using AutoMapper;

namespace Application.Command
{
    public class AddPlayerToDepthChartRequest : IRequest<Unit>
    {
        public DepthChartEntryDto DepthChartEntry { get; set; }
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
            var entries = await _queryRepository.GetDepthChartEntriesAsync(request.DepthChartEntry.TeamId);
            int depth = request.DepthChartEntry.PositionDepth ?? entries.Count(e => e.Position == request.DepthChartEntry.Position);

            if (request.DepthChartEntry.PositionDepth.HasValue)
            {
                foreach (var entry in entries.Where(e => e.Position == request.DepthChartEntry.Position && e.PositionDepth >= request.DepthChartEntry.PositionDepth.Value))
                {
                    entry.PositionDepth++;
                }
            }

            var depthChartEntry = _mapper.Map<DepthChartEntry>(request.DepthChartEntry);
            depthChartEntry.PositionDepth = depth;
            await _commandRepository.AddPlayerToDepthChartAsync(depthChartEntry);
            return Unit.Value;
        }
    }
}
