using Microsoft.AspNetCore.Mvc;
using RapidOrder.Core.Services;

namespace RapidOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("track-served-mission")]
    public async Task<ActionResult<TrackServedMissionResponse>> GetTrackServedMission(CancellationToken cancellationToken)
    {
        var value = await _settingsService.GetTrackServedMissionAsync(cancellationToken);
        return Ok(new TrackServedMissionResponse(value));
    }

    [HttpPut("track-served-mission")]
    public async Task<IActionResult> SetTrackServedMission(
        [FromBody] TrackServedMissionRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest();
        }

        await _settingsService.SetTrackServedMissionAsync(request.TrackServedMission, cancellationToken);
        return NoContent();
    }

    public sealed record TrackServedMissionResponse(bool TrackServedMission);

    public sealed record TrackServedMissionRequest(bool TrackServedMission);
}
