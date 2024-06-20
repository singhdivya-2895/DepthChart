
using Application.Command;
using Application.DTO;
using Application.Mapping;
using Application.Query;
using Application.Validations;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.IRepository;
using Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FanDuelMemoryDbContext>(options => options.UseInMemoryDatabase("DepthChartDB"));
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();

            builder.Services.AddAutoMapper(typeof(DepthChartProfile));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(AddPlayerToDepthChartRequest)));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Depth Chart Api", Version = "v1" });
            });
            builder.Services.AddScoped<IValidator<TeamDto>, TeamDtoValidator>();
            builder.Services.AddScoped<IValidator<DepthChartEntryDto>, DepthChartEntryDtoValidator>();

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();
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

            app.MapPost("/api/team", async (IValidator<TeamDto> validator,IMediator mediator, [FromBody] TeamDto team) =>
            {
                ValidationResult validationResult = await validator.ValidateAsync(team);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
                var teamDto = await mediator.Send(new AddTeamRequest
                {
                    TeamDto = team
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

            app.MapPost("/api/depthchartentry", async (IValidator<DepthChartEntryDto> validator, IMediator mediator, [FromBody] DepthChartEntryDto depthChartRequest) =>
            {
                ValidationResult validationResult = await validator.ValidateAsync(depthChartRequest);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
                var (statusCode, message) = await mediator.Send(new AddPlayerToDepthChartRequest
                {
                    DepthChartEntry = depthChartRequest
                });
                if (!statusCode)
                {
                    return Results.NotFound(message);
                }

                return Results.Ok();
            })
            .WithName("AddPlayerToDepthChart")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Add player to depth chart",
                Description = "POST endpoint to add a player to the depth chart for respective team.",
                Tags = new List<OpenApiTag> { new() { Name = "Command" } }
            });

            app.MapDelete("/api/depthchartentry/{teamId}", async (IMediator mediator, [FromRoute] string teamId, string position, int playerNumber) =>
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

            app.MapGet("/api/depthchart/{teamId}/backups", async (IMediator mediator, [FromRoute] string teamId, string position, int playerNumber) =>
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

            app.MapGet("/api/depthchart/{teamId}", async (IMediator mediator, [FromRoute] string teamId) =>
            {
                var depthChart = await mediator.Send(new GetFullDepthChartRequest
                {
                    TeamId = teamId
                });
                if (depthChart is null)
                {
                    return Results.NotFound("Team does not exist for the Id.");
                }

                return Results.Ok(depthChart);
            })
            .WithName("GetFullDepthChart")
            .Produces<Dictionary<string, List<FullDepthChartEntryDto>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Summary = "Get full depth chart",
                Description = "GET endpoint to retrieve full depth chart for a team.",
                Tags = new List<OpenApiTag> { new() { Name = "Query" } }
            });

            app.MapGet("/api/teams/{sport}", async (IMediator mediator, [FromRoute] Sport sport) =>
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


                dbContext.DepthChartEntries.AddRange(
                    new List<DepthChartEntry>
                       {
                           new DepthChartEntry { TeamId = "A", Position = "LT", PositionDepth = 0, Player = new Player(){ Name = "Player 1", Number = 1} },
                           new DepthChartEntry { TeamId = "A", Position = "LT", PositionDepth = 1, Player = new Player(){ Name = "Player 4", Number = 4} }
                       });

                dbContext.DepthChartEntries.AddRange(
                    new List<DepthChartEntry>
                       {
                           new DepthChartEntry { TeamId = "A", Position = "LG", PositionDepth = 0, Player = new Player(){ Name = "Player 5", Number = 5} }
                       });

                dbContext.DepthChartEntries.AddRange(
                    new List<DepthChartEntry>
                       {
                           new DepthChartEntry { TeamId = "A", Position = "RB", PositionDepth = 0, Player = new Player(){ Name = "Player 6", Number = 6} }
                       });

                dbContext.SaveChanges();
            }
        }
    }
}
