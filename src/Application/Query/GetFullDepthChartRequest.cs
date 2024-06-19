using Application.DTO;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GetFullDepthChartHandler> _logger;

        public GetFullDepthChartHandler(ITeamRepository teamRepository, IMapper mapper, ILogger<GetFullDepthChartHandler> logger)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Dictionary<string, List<FullDepthChartEntryDto>>> Handle(GetFullDepthChartRequest request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);

            if (team == null)
            {
                _logger.LogError($"Team does not exist for Id: {request.TeamId}.");
                return null;
            }

            var result = team.DepthChartEntries.GroupBy(d => d.Position)
                                          .ToDictionary(g => g.Key,
                                                        g => g.OrderBy(e => e.PositionDepth)
                                                             .Select(e => _mapper.Map<FullDepthChartEntryDto>(e))
                                                             .ToList());


            _logger.LogInformation("Full Depth chart created successfully.");

            return result;
        }
    }
}
