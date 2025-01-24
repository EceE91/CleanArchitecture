using FluentValidation;
using FluentValidation.Validators;
using CleanArchitecture.Locations.Application.Commands;
using CleanArchitecture.Locations.Domain.Interfaces;

namespace CleanArchitecture.Locations.Application.Handlers;

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator(ILocationRepository locationRepository)
    {
        RuleFor(x => x.Id)
            .SetAsyncValidator(new AsyncPredicateValidator<UpdateLocationCommand, int>(async (_, id, _, cancellationToken) => await locationRepository.GetLocationByIdAsync(id, cancellationToken) != null))
            .WithMessage("Location does not exist.")
            .WithErrorCode(ValidationErrorCodes.EntityNotFound);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        
    }
}