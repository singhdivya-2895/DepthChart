
using Application.Command;
using Application.DTO;
using Application.Mapping;
using Application.Query;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.IRepository;
using Persistence.Repository;
using System;
using System.Data;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FanDuelMemoryDbContext>(options => options.UseInMemoryDatabase("DepthChartDB"));
            builder.Services.AddScoped<IDepthChartCommandRepository, DepthChartCommandRepository>();
            builder.Services.AddScoped<IDepthChartQueryRepository, DepthChartQueryRepository>();

            builder.Services.AddAutoMapper(typeof(DepthChartProfile));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(AddPlayerToDepthChartRequest)));
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
                        
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FanDuelMemoryDbContext>();
                SeedInitialData(dbContext);
            }
            app.UseHttpsRedirection();

            app.MapPost("/api/team/add", async (IMediator mediator, string name, Sport sport) =>
            {
                var teamDto = await mediator.Send(new AddTeamRequest
                {
                    Name = name,
                    Sport = sport
                });

                return Results.Ok(teamDto);
            })
            .WithName("AddTeamToSport")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Add Team to Sport",
                Description = "POST endpoint to add a new team to sport.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapPost("/api/depthchart/add", async (IMediator mediator, int teamId, string position, PlayerDto player, int? positionDepth) =>
            {
                await mediator.Send(new AddPlayerToDepthChartRequest
                {
                    TeamId = teamId,
                    Position = position,
                    Player = player,
                    PositionDepth = positionDepth
                });

                return Results.Ok();
            })
            .WithName("AddPlayerToDepthChart")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Add player to depth chart",
                Description = "POST endpoint to add a player to the depth chart for respective team.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapDelete("/api/depthchart/remove", async (IMediator mediator, int teamId, string position, int playerNumber) =>
            {
                var player = await mediator.Send(new RemovePlayerFromDepthChartRequest
                {
                    TeamId = teamId,
                    Position = position,
                    PlayerNumber = playerNumber
                });

                return player == null ? Results.NotFound() : Results.Ok(player);
            })
            .WithName("RemovePlayerFromDepthChart")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Remove player from the depth chart",
                Description = "DELETE endpoint to remove a player from the depth chart.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapGet("/api/depthchart/backups", async (IMediator mediator, int teamId, string position, int playerNumber) =>
            {
                var backups = await mediator.Send(new GetBackupsRequest
                {
                    TeamId = teamId,
                    Position = position,
                    PlayerNumber = playerNumber
                });

                return Results.Ok(backups);
            })
            .WithName("GetBackups")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get backups for player",
                Description = "GET endpoint to retrieve the backups for a player and position.",
                Tags = new List<OpenApiTag> { new() { Name = "Query" } }
            });

            app.MapGet("/api/depthchart/full", async (IMediator mediator, int teamId) =>
            {
                var depthChart = await mediator.Send(new GetFullDepthChartRequest
                {
                    TeamId = teamId
                });

                return Results.Ok(depthChart);
            })
            .WithName("GetFullDepthChart")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get full depth chart",
                Description = "GET endpoint to retrieve full depth chart for a team.",
                Tags = new List<OpenApiTag> { new() { Name = "Query" } }
            });

            app.MapGet("/api/teams", async (IMediator mediator, Sport sport) =>
            {
                var teams = await mediator.Send(new GetTeamsBySportRequest
                {
                    Sport = sport
                });

                return Results.Ok(teams);
            })
            .WithName("GetTeamsForSport")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get all teams for sport",
                Description = "GET endpoint to retrieve all teams for the sport.",
                Tags = new List<OpenApiTag> { new() { Name = "Query" } }
            });


            app.Run();
            void SeedInitialData(FanDuelMemoryDbContext dbContext)
            {
                dbContext.Teams.AddRange(
                    new Team { Name = "Team A", Sport = Sport.NFL },
                    new Team { Name = "Team B", Sport = Sport.NFL }
                );

                dbContext.SaveChanges();
            }
        }
    }
}
