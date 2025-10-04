using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Api.Validation;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Places;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum;

[ApiController]
[Authorize]
[Route("nigrum/places")]
public class PlacesController : ControllerBase
{
    private readonly IPlaceAssignmentService _placeAssignmentService;
    private readonly IValidator<AssignPlaceRequest> _assignPlaceValidator;

    public PlacesController(IPlaceAssignmentService placeAssignmentService, IValidator<AssignPlaceRequest> assignPlaceValidator)
    {
        _placeAssignmentService = placeAssignmentService;
        _assignPlaceValidator = assignPlaceValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaceSummary>>> GetPlaces([FromQuery] PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var result = await _placeAssignmentService.GetPlacesAsync(pagination, cancellationToken);

        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        Response.Headers["X-Page-Number"] = result.PageNumber.ToString();
        Response.Headers["X-Page-Size"] = result.PageSize.ToString();

        return Ok(result.Items);
    }

    [HttpPut("{placeId:int}/employee/{employeeId:long}")]
    public async Task<IActionResult> AssignPlace([FromRoute] AssignPlaceRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _assignPlaceValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult.ToModelStateDictionary());
        }

        try
        {
            await _placeAssignmentService.AssignPlaceToEmployeeAsync(request.PlaceId, request.EmployeeId, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
