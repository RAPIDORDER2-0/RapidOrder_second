using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Api.Validation;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Watches;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum;

[ApiController]
[Authorize]
[Route("nigrum/watches")]
public class WatchesController : ControllerBase
{
    private readonly IWatchService _watchService;
    private readonly IValidator<AssignWatchRequest> _assignValidator;

    public WatchesController(IWatchService watchService, IValidator<AssignWatchRequest> assignValidator)
    {
        _watchService = watchService;
        _assignValidator = assignValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WatchSummary>>> GetWatches([FromQuery] PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var result = await _watchService.GetAllWatchesAsync(pagination, cancellationToken);

        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
        Response.Headers["X-Page-Size"] = result.PageSize.ToString();

        return Ok(result.Items);
    }

    [HttpPut("{watchId:int}/employee/{employeeId:long}")]
    public async Task<IActionResult> AssignWatch([FromRoute] AssignWatchRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _assignValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult.ToModelStateDictionary());
        }

        try
        {
            await _watchService.AssignWatchAsync(request.WatchId, request.EmployeeId, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("{watchId:int}/employee/unassign")]
    public async Task<IActionResult> UnassignWatch(int watchId, CancellationToken cancellationToken)
    {
        if (!await _watchService.WatchExistsAsync(watchId, cancellationToken))
        {
            return NotFound();
        }

        await _watchService.UnassignWatchAsync(watchId, cancellationToken);
        return NoContent();
    }
}
