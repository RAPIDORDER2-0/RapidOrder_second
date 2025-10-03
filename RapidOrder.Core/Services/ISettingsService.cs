namespace RapidOrder.Core.Services;

public interface ISettingsService
{
    Task<bool> GetTrackServedMissionAsync(CancellationToken cancellationToken = default);
    Task SetTrackServedMissionAsync(bool value, CancellationToken cancellationToken = default);
}
