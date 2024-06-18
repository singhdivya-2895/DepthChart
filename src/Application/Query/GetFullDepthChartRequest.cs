using Application.DTO;
using AutoMapper;
using MediatR;
using Persistence.IRepository;

namespace Application.Query
{
    public class GetFullDepthChartRequest : IRequest<Dictionary<string, List<DepthChartEntryDto>>>
    {
        public int TeamId { get; set; }
    }
    public class GetFullDepthChartHandler : IRequestHandler<GetFullDepthChartRequest, Dictionary<string, List<DepthChartEntryDto>>>
    {
        private readonly IDepthChartQueryRepository _queryRepository;
        private readonly IMapper _mapper;

        public GetFullDepthChartHandler(IDepthChartQueryRepository queryRepository, IMapper mapper)
        {
            _queryRepository = queryRepository;
            _mapper = mapper;
        }

        public async Task<Dictionary<string, List<DepthChartEntryDto>>> Handle(GetFullDepthChartRequest request, CancellationToken cancellationToken)
        {
            var depthChartEntries = await _queryRepository.GetDepthChartEntriesAsync(request.TeamId);

            var result = depthChartEntries.GroupBy(d => d.Position)
                                          .ToDictionary(g => g.Key,
                                                        g => g.Select(e => _mapper.Map<DepthChartEntryDto>(e)).ToList());

            return result;
        }
    }
}
