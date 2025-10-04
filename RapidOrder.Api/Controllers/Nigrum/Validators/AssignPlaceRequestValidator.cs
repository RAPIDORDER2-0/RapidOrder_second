using FluentValidation;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum.Validators;

public class AssignPlaceRequestValidator : AbstractValidator<AssignPlaceRequest>
{
    public AssignPlaceRequestValidator(IPlaceAssignmentService placeAssignmentService, IEmployeeService employeeService)
    {
        RuleFor(x => x.PlaceId)
            .GreaterThan(0)
            .MustAsync(async (placeId, cancellationToken) =>
                await placeAssignmentService.PlaceExistsAsync(placeId, cancellationToken))
            .WithMessage("Place does not exist.");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .MustAsync(async (employeeId, cancellationToken) =>
                await employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
            .WithMessage("Employee does not exist.");
    }
}
