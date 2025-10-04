using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Api.Validation;
using RapidOrder.Core.Models.Employees;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Places;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum;

[ApiController]
[Authorize]
[Route("nigrum/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IValidator<EmployeeBreakCommand> _breakValidator;

    public EmployeesController(IEmployeeService employeeService, IValidator<EmployeeBreakCommand> breakValidator)
    {
        _employeeService = employeeService;
        _breakValidator = breakValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeSummary>>> GetEmployees([FromQuery] PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetAllEmployeesAsync(pagination, cancellationToken);

        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
        Response.Headers["X-Page-Size"] = result.PageSize.ToString();

        return Ok(result.Items);
    }

    [HttpGet("{id:long}/places")]
    public async Task<ActionResult<IEnumerable<PlaceSummary>>> GetPlacesForEmployee(long id, CancellationToken cancellationToken)
    {
        if (!await _employeeService.EmployeeExistsAsync(id, cancellationToken))
        {
            return NotFound();
        }

        var places = await _employeeService.GetPlacesForEmployeeAsync(id, cancellationToken);
        return Ok(places);
    }

    [HttpPut("{id:long}/break")]
    public async Task<IActionResult> SetBreak(long id, [FromBody] EmployeeBreakRequest? request, CancellationToken cancellationToken)
    {
        var command = new EmployeeBreakCommand(id, request);
        var validationResult = await _breakValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult.ToModelStateDictionary());
        }

        try
        {
            await _employeeService.SetBreakAsync(id, command.Payload!.IsOnBreak, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return NoContent();
    }
}
