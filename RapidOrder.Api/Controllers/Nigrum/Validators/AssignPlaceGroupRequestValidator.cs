using FluentValidation;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum.Validators;

public class AssignPlaceGroupRequestValidator : AbstractValidator<AssignPlaceGroupRequest>
{
    public AssignPlaceGroupRequestValidator(IPlaceAssignmentService placeAssignmentService, IEmployeeService employeeService)
    {
        RuleFor(x => x.PlaceGroupId)
            .GreaterThan(0)
            .MustAsync(async (placeGroupId, cancellationToken) =>
                await placeAssignmentService.PlaceGroupExistsAsync(placeGroupId, cancellationToken))
            .WithMessage("Place group does not exist.");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .MustAsync(async (employeeId, cancellationToken) =>
                await employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
            .WithMessage("Employee does not exist.");
    }
}
