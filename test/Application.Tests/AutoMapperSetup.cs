using Application.DTO;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests
{
    public static class AutoMapperSetup
    {
        public static IMapper Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DepthChartEntry, DepthChartEntryDto>().ReverseMap();
                cfg.CreateMap<Player, PlayerDto>().ReverseMap();
                cfg.CreateMap<Team, TeamDto>().ReverseMap();
            });

            return config.CreateMapper();
        }
    }

}
