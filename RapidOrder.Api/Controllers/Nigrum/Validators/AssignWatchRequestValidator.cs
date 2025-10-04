using FluentValidation;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum.Validators;

public class AssignWatchRequestValidator : AbstractValidator<AssignWatchRequest>
{
    public AssignWatchRequestValidator(IWatchService watchService, IEmployeeService employeeService)
    {
        RuleFor(x => x.WatchId)
            .GreaterThan(0)
            .MustAsync(async (watchId, cancellationToken) =>
                await watchService.WatchExistsAsync(watchId, cancellationToken))
            .WithMessage("Watch does not exist.");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .MustAsync(async (employeeId, cancellationToken) =>
                await employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
            .WithMessage("Employee does not exist.");
    }
}
