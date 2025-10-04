using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RapidOrder.Core.Constants;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Options;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class SettingsService : ISettingsService
{
    private readonly RapidOrderDbContext _dbContext;
    private readonly ILogger<SettingsService> _logger;
    private readonly MissionServiceOptions _defaults;

    public SettingsService(
        RapidOrderDbContext dbContext,
        ILogger<SettingsService> logger,
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
            .FirstOrDefaultAsync(s => s.Name == SettingKeys.TrackServedMission, cancellationToken);

        if (setting?.Value is null)
        {
            return _defaults.TrackServeMission;
        }

        return setting.Value == "1" || setting.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task SetTrackServedMissionAsync(bool value, CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.Settings
            .FirstOrDefaultAsync(s => s.Name == SettingKeys.TrackServedMission, cancellationToken);

        if (setting == null)
        {
            setting = new Setting
            {
                Name = SettingKeys.TrackServedMission,
                Value = value ? "1" : "0"
            };

            _dbContext.Settings.Add(setting);
        }
        else
        {
            setting.Value = value ? "1" : "0";
        }

        _logger.LogInformation("Setting {Setting} updated to {Value}", SettingKeys.TrackServedMission, setting.Value);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
