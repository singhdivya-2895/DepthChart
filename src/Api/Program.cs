
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Depth Chart Api", Version = "v1" });
            });

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

            app.MapPost("/api/team", async (IMediator mediator, TeamDto team) =>
            {
                var teamDto = await mediator.Send(new AddTeamRequest
                {
                    teamDto = team
                });

                return Results.Ok(teamDto);
            })
            .WithName("AddTeamToSport")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Add Team to Sport",
                Description = "POST endpoint to add a new team to sport.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapPost("/api/depthchart", async (IMediator mediator, DepthChartEntryDto depthChartRequest) =>
            {
                await mediator.Send(new AddPlayerToDepthChartRequest
                {
                    DepthChartEntry = depthChartRequest
                });

                return Results.Ok();
            })
            .WithName("AddPlayerToDepthChart")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Add player to depth chart",
                Description = "POST endpoint to add a player to the depth chart for respective team.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapDelete("/api/depthchart", async (IMediator mediator, string teamId, string position, int playerNumber) =>
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
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Remove player from the depth chart",
                Description = "DELETE endpoint to remove a player from the depth chart.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapGet("/api/depthchart/backups", async (IMediator mediator, string teamId, string position, int playerNumber) =>
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
            .Produces<List<PlayerDto>>(StatusCodes.Status200OK)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get backups for player",
                Description = "GET endpoint to retrieve the backups for a player and position.",
                Tags = new List<OpenApiTag> { new() { Name = "Query" } }
            });

            app.MapGet("/api/depthchart/full", async (IMediator mediator, string teamId) =>
            {
                var depthChart = await mediator.Send(new GetFullDepthChartRequest
                {
                    TeamId = teamId
                });
                if (!depthChart.Any())
                {
                    return Results.NotFound("Team does not exist for the Id.");
                }

                return Results.Ok(depthChart);
            })
            .WithName("GetFullDepthChart")
            .Produces<Dictionary<string, List<DepthChartEntryDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
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
                if (!teams.Any())
                {
                    return Results.NotFound("No teams exist for this sport.");
                }

                return Results.Ok(teams);
            })
            .WithName("GetTeamsForSport")
            .Produces<List<TeamDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
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
                    new Team { Id = "A", Name = "Team A", Sport = Sport.NFL },
                    new Team { Id = "B", Name = "Team B", Sport = Sport.NFL }
                );

                dbContext.DepthChartEntries.AddRange(
                    new List<DepthChartEntry>
                       {
                           new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 0, Player = new Player(){ Name = "Player 1", Number = 1} },
                           new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 1, Player = new Player(){ Name = "Player 2", Number = 2} },
                           new DepthChartEntry { TeamId = "A", Position = "QB", PositionDepth = 2, Player = new Player(){ Name = "Player 3", Number = 3} }
                       });

                dbContext.SaveChanges();
            }
        }
    }
}
