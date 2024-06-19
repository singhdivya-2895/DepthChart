using Application.DTO;
using AutoMapper;
using Domain.Models;

namespace Application.Tests
{
    public static class AutoMapperSetup
    {
        public static IMapper Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DepthChartEntry, DepthChartEntryDto>().ReverseMap();
                cfg.CreateMap<DepthChartEntry, FullDepthChartEntryDto>();
                cfg.CreateMap<Player, PlayerDto>().ReverseMap();
                cfg.CreateMap<Team, TeamDto>().ReverseMap();
            });

            return config.CreateMapper();
        }
    }

}
