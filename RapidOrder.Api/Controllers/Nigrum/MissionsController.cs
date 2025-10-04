using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers.Nigrum;

[ApiController]
[Authorize]
[Route("nigrum/missions")]
public class MissionsController : ControllerBase
{
    private readonly IMissionService _missionService;

    public MissionsController(IMissionService missionService)
    {
        _missionService = missionService;
    }

    [HttpPut("close")]
    public async Task<IActionResult> CloseOpenMissions(CancellationToken cancellationToken)
    {
        await _missionService.CancelAllOpenMissionsAsync(cancellationToken: cancellationToken);
        return NoContent();
    }
}
