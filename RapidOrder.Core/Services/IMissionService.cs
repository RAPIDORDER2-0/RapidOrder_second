using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Services;

public interface IMissionService
{
    Task<MissionStartResult> StartMissionAsync(
        int? placeId,
        MissionType type,
        long? userId = null,
        DateTime? startedAt = null,
        string? sourceDecoded = null,
        int? sourceButton = null,
        CancellationToken cancellationToken = default);

    Task<Mission?> FinishMissionAsync(
        long missionId,
        long? userId = null,
        DateTime? finishedAt = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Mission>> FinishPlaceMissionsAsync(
        int placeId,
        long? userId = null,
        DateTime? finishedAt = null,
        CancellationToken cancellationToken = default);

    Task<Mission?> CancelMissionAsync(
        long missionId,
        DateTime? canceledAt = null,
        CancellationToken cancellationToken = default);

    Task<int> CancelPlaceMissionsAsync(
        int placeId,
        DateTime? canceledAt = null,
        CancellationToken cancellationToken = default);

    Task<int> CancelAllOpenMissionsAsync(
        DateTime? canceledAt = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Mission>> GetStartedMissionsByPlaceAsync(
        int placeId,
        CancellationToken cancellationToken = default);
}
