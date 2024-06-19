using Application.Command;
using Application.DTO;
using FluentValidation;

namespace Application.Validations
{
    public class DepthChartEntryDtoValidator : AbstractValidator<DepthChartEntryDto>
    {
        public DepthChartEntryDtoValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty();
            RuleFor(x => x.Position).NotEmpty();
            RuleFor(x => x.Player).NotNull().SetValidator(new PlayerDtoValidator());
        }
    }

    public class PlayerDtoValidator : AbstractValidator<PlayerDto>
    {
        public PlayerDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Number).GreaterThan(0);
        }
    }

    public class TeamDtoValidator : AbstractValidator<TeamDto>
    {
        public TeamDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Sport).IsInEnum();
        }
    }

}
