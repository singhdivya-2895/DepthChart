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
        private readonly IDepthChartQueryRepository _queryRepository;

        private readonly IMapper _mapper;

        public GetBackupsHandler(IDepthChartQueryRepository queryRepository, IMapper mapper)
        {
            _queryRepository = queryRepository;
            _mapper = mapper;
        }

        public async Task<List<PlayerDto>> Handle(GetBackupsRequest request, CancellationToken cancellationToken)
        {
            var entry = await _queryRepository.GetDepthChartEntryAsync(request.TeamId, request.Position, request.PlayerNumber);
            if (entry == null) return new List<PlayerDto>();

            var entries = await _queryRepository.GetDepthChartEntriesAsync(request.TeamId);
            var backups = entries.Where(e => e.Position == request.Position && e.PositionDepth > entry.PositionDepth)
                                 .Select(e => _mapper.Map<PlayerDto>(e.Player))
                                 .ToList();

            return backups;
        }
    }
}
