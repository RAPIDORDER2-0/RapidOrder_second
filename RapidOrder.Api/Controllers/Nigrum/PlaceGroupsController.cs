using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidOrder.Api.Controllers.Nigrum.Requests;
using RapidOrder.Api.Validation;
using RapidOrder.Core.Models.PlaceGroups;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum;

[ApiController]
[Authorize]
[Route("nigrum/place-groups")]
public class PlaceGroupsController : ControllerBase
{
    private readonly IPlaceAssignmentService _placeAssignmentService;
    private readonly IValidator<AssignPlaceGroupRequest> _assignValidator;

    public PlaceGroupsController(IPlaceAssignmentService placeAssignmentService, IValidator<AssignPlaceGroupRequest> assignValidator)
    {
        _placeAssignmentService = placeAssignmentService;
        _assignValidator = assignValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaceGroupSummary>>> GetAll(CancellationToken cancellationToken)
    {
        var groups = await _placeAssignmentService.GetPlaceGroupsAsync(cancellationToken);
        return Ok(groups);
    }

    [HttpPut("{placeGroupId:int}/employee/{employeeId:long}")]
    public async Task<IActionResult> AssignPlaceGroup([FromRoute] AssignPlaceGroupRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _assignValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult.ToModelStateDictionary());
        }

        try
        {
            await _placeAssignmentService.AssignPlaceGroupToEmployeeAsync(request.PlaceGroupId, request.EmployeeId, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return NoContent();
    }
}
