using Application.DTO;
using AutoMapper;
using MediatR;
using Persistence.IRepository;

namespace Application.Command
{
    public class RemovePlayerFromDepthChartRequest : IRequest<PlayerDto>
    {
        public int TeamId { get; set; }
        public string Position { get; set; }
        public int PlayerNumber { get; set; }
    }

    public class RemovePlayerFromDepthChartHandler : IRequestHandler<RemovePlayerFromDepthChartRequest, PlayerDto>
    {
        private readonly IDepthChartCommandRepository _commandRepository;
        private readonly IDepthChartQueryRepository _queryRepository;
        private readonly IMapper _mapper;

        public RemovePlayerFromDepthChartHandler(IDepthChartCommandRepository commandRepository, IDepthChartQueryRepository queryRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _mapper = mapper;
        }

        public async Task<PlayerDto> Handle(RemovePlayerFromDepthChartRequest request, CancellationToken cancellationToken)
        {
            var entry = await _queryRepository.GetDepthChartEntryAsync(request.TeamId, request.Position, request.PlayerNumber);
            if (entry == null) return null;

            await _commandRepository.RemovePlayerFromDepthChartAsync(entry);

            return _mapper.Map<PlayerDto>(entry.Player);
        }
    }
}
