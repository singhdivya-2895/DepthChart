using Application.DTO;
using AutoMapper;
using MediatR;
using Persistence.IRepository;

namespace Application.Query
{
    public class GetFullDepthChartRequest : IRequest<Dictionary<string, List<FullDepthChartEntryDto>>>
    {
        public string TeamId { get; set; }
    }
    public class GetFullDepthChartHandler : IRequestHandler<GetFullDepthChartRequest, Dictionary<string, List<FullDepthChartEntryDto>>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public GetFullDepthChartHandler(ITeamRepository teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<Dictionary<string, List<FullDepthChartEntryDto>>> Handle(GetFullDepthChartRequest request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);

            if (team == null) return null;

            var result = team.DepthChartEntries.GroupBy(d => d.Position)
                                          .ToDictionary(g => g.Key,
                                                        g => g.OrderBy(e => e.PositionDepth)
                                                             .Select(e => _mapper.Map<FullDepthChartEntryDto>(e))
                                                             .ToList())
                                          ;

            return result;
        }
    }
}
