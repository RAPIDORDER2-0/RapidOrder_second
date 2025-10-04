using FluentValidation;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum.Validators;

public class EmployeeBreakCommandValidator : AbstractValidator<EmployeeBreakCommand>
{
    public EmployeeBreakCommandValidator(IEmployeeService employeeService)
    {
        RuleFor(x => x.Payload)
            .NotNull();

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .MustAsync(async (employeeId, cancellationToken) =>
                await employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
            .WithMessage("Employee does not exist.");
    }
}
