using Application.DTO;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping
{
    public class DepthChartProfile : Profile
    {
        public DepthChartProfile()
        {
            CreateMap<Player, PlayerDto>().ReverseMap();
            CreateMap<Team, TeamDto>().ReverseMap();
            CreateMap<DepthChartEntry, DepthChartEntryDto>().ReverseMap();
        }
    }
}
