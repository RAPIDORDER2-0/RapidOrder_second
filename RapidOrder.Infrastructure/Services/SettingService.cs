using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Options;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class SettingService : ISettingsService
{
    private const string TrackServedMissionKey = "trackServedMission";

    private readonly RapidOrderDbContext _dbContext;
    private readonly ILogger<SettingService> _logger;
    private readonly MissionServiceOptions _defaults;

    public SettingService(
        RapidOrderDbContext dbContext,
        ILogger<SettingService> logger,
        IOptions<MissionServiceOptions> defaults)
    {
        _dbContext = dbContext;
        _logger = logger;
        _defaults = defaults.Value;
    }

    public async Task<bool> GetTrackServedMissionAsync(CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == TrackServedMissionKey, cancellationToken);

        if (setting?.Value is null)
        {
            return _defaults.TrackServeMission;
        }

        return setting.Value == "1" || setting.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task SetTrackServedMissionAsync(bool value, CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.Settings
            .FirstOrDefaultAsync(s => s.Name == TrackServedMissionKey, cancellationToken);

        if (setting == null)
        {
            setting = new Setting
            {
                Name = TrackServedMissionKey,
                Value = value ? "1" : "0"
            };

            _dbContext.Settings.Add(setting);
        }
        else
        {
            setting.Value = value ? "1" : "0";
        }

        _logger.LogInformation("Setting {Setting} updated to {Value}", TrackServedMissionKey, setting.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
