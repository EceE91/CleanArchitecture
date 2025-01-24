using FluentValidation;
using CleanArchitecture.Locations.Application.Commands;

namespace CleanArchitecture.Locations.Application.Handlers;

public class AddLocationCommandValidator : AbstractValidator<AddLocationCommand>
{
    public AddLocationCommandValidator()
    {
        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
    }
}