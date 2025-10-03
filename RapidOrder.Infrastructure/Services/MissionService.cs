using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Core.Options;
using RapidOrder.Core.Services;

#pragma warning disable CS8602

namespace RapidOrder.Infrastructure.Services;

public class MissionService : IMissionService
{
    private readonly RapidOrderDbContext _dbContext;
    private readonly ILogger<MissionService> _logger;
    private readonly MissionServiceOptions _options;

    public MissionService(
        RapidOrderDbContext dbContext,
        ILogger<MissionService> logger,
        IOptions<MissionServiceOptions> options)
    {
        _dbContext = dbContext;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<MissionStartResult> StartMissionAsync(
        int? placeId,
        MissionType type,
        long? userId = null,
        DateTime? startedAt = null,
        string? sourceDecoded = null,
        int? sourceButton = null,
        CancellationToken cancellationToken = default)
    {
        var timestamp = startedAt?.ToUniversalTime() ?? DateTime.UtcNow;

        var query = _dbContext.Missions.AsQueryable()
            .Where(m => m.Status == MissionStatus.STARTED && m.Type == type);

        if (placeId.HasValue)
        {
            query = query.Where(m => m.PlaceId == placeId);
        }

        if (userId.HasValue)
        {
            query = query.Where(m => m.AssignedUserId == userId);
        }

        var existingMission = await query.FirstOrDefaultAsync(cancellationToken);
        if (existingMission != null)
        {
            _logger.LogInformation("Mission already running for place {PlaceId} and type {Type}; returning existing mission {MissionId}", placeId, type, existingMission.Id);
            return new MissionStartResult(existingMission, false);
        }

        Place? place = null;
        if (placeId.HasValue)
        {
            place = await _dbContext.Places
                .Include(p => p.PlaceGroup)
                .Include(p => p.Setup)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == placeId.Value, cancellationToken);

            if (place == null)
            {
                throw new InvalidOperationException($"Place {placeId.Value} was not found");
            }
        }

        long? effectiveUserId = userId ?? place?.UserId;
        User? assignedUser = null;
        if (effectiveUserId.HasValue)
        {
            if (place?.User != null && place.UserId == effectiveUserId)
            {
                assignedUser = place.User;
            }
            else
            {
                assignedUser = await _dbContext.Users.FindAsync(new object?[] { effectiveUserId.Value }, cancellationToken);
            }
        }

        var mission = new Mission
        {
            Type = type,
            Status = MissionStatus.STARTED,
            StartedAt = timestamp,
            PlaceId = placeId,
            Place = place,
            PlaceGroupId = place?.PlaceGroupId,
            PlaceGroup = place?.PlaceGroup,
            SetupId = place?.SetupId,
            Setup = place?.Setup,
            AssignedUserId = effectiveUserId,
            AssignedUser = assignedUser,
            SourceDecoded = sourceDecoded,
            SourceButton = sourceButton,
            IdleTimeSeconds = 0
        };

        if (place != null)
        {
            mission.IdleTimeSeconds = await CalculateIdleTimeSecondsAsync(place, type, timestamp, cancellationToken) ?? 0;
        }

        _dbContext.Missions.Add(mission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Started mission {MissionId} for place {PlaceId} with type {Type}", mission.Id, placeId, type);
        return new MissionStartResult(mission, true);
    }

    public async Task<Mission?> FinishMissionAsync(
        long missionId,
        long? userId = null,
        DateTime? finishedAt = null,
        CancellationToken cancellationToken = default)
    {
        var mission = await _dbContext.Missions
            .Include(m => m.Place)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == missionId, cancellationToken);

        if (mission == null)
        {
            return null;
        }

        var missionEntity = mission!;
        await StopMissionAsync(missionEntity, MissionStatus.FINISHED, finishedAt, userId, true, cancellationToken);

        missionEntity.FinishedAt ??= DateTime.UtcNow;
        if (missionEntity.FinishedAt.HasValue)
        {
            await HandleServeTrackingAsync(missionEntity, missionEntity.FinishedAt.Value, cancellationToken);
        }

        return missionEntity;
    }

    public async Task<IReadOnlyList<Mission>> FinishPlaceMissionsAsync(
        int placeId,
        long? userId = null,
        DateTime? finishedAt = null,
        CancellationToken cancellationToken = default)
    {
        var missions = await _dbContext.Missions
            .Include(m => m.Place)
                .ThenInclude(p => p.User)
            .Where(m => m.PlaceId == placeId && m.Status == MissionStatus.STARTED)
            .ToListAsync(cancellationToken);

        if (missions.Count == 0)
        {
            return Array.Empty<Mission>();
        }

        var when = finishedAt?.ToUniversalTime() ?? DateTime.UtcNow;

        foreach (var mission in missions)
        {
            await StopMissionAsync(mission, MissionStatus.FINISHED, when, userId, saveImmediately: false, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var mission in missions)
        {
            mission.FinishedAt ??= when;
            if (mission.FinishedAt.HasValue)
            {
                await HandleServeTrackingAsync(mission, mission.FinishedAt.Value, cancellationToken);
            }
        }

        return missions;
    }

    public async Task<Mission?> CancelMissionAsync(
        long missionId,
        DateTime? canceledAt = null,
        CancellationToken cancellationToken = default)
    {
        var mission = await _dbContext.Missions
            .Include(m => m.Place)
            .FirstOrDefaultAsync(m => m.Id == missionId && m.Status == MissionStatus.STARTED, cancellationToken);

        if (mission == null)
        {
            return null;
        }

        await StopMissionAsync(mission, MissionStatus.CANCELED, canceledAt, userId: null, saveImmediately: true, cancellationToken);
        return mission;
    }

    public async Task<int> CancelPlaceMissionsAsync(
        int placeId,
        DateTime? canceledAt = null,
        CancellationToken cancellationToken = default)
    {
        var missions = await _dbContext.Missions
            .Where(m => m.PlaceId == placeId && m.Status == MissionStatus.STARTED)
            .ToListAsync(cancellationToken);

        if (missions.Count == 0)
        {
            return 0;
        }

        var when = canceledAt?.ToUniversalTime() ?? DateTime.UtcNow;

        foreach (var mission in missions)
        {
            await StopMissionAsync(mission, MissionStatus.CANCELED, when, userId: null, saveImmediately: false, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return missions.Count;
    }

    public async Task<int> CancelAllOpenMissionsAsync(
        DateTime? canceledAt = null,
        CancellationToken cancellationToken = default)
    {
        var missions = await _dbContext.Missions
            .Where(m => m.Status == MissionStatus.STARTED)
            .ToListAsync(cancellationToken);

        if (missions.Count == 0)
        {
            return 0;
        }

        var when = canceledAt?.ToUniversalTime() ?? DateTime.UtcNow;

        foreach (var mission in missions)
        {
            await StopMissionAsync(mission, MissionStatus.CANCELED, when, userId: null, saveImmediately: false, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return missions.Count;
    }

    public async Task<IReadOnlyList<Mission>> GetStartedMissionsByPlaceAsync(
        int placeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Missions
            .AsNoTracking()
            .Where(m => m.PlaceId == placeId && m.Status == MissionStatus.STARTED)
            .ToListAsync(cancellationToken);
    }

    private async Task<long?> CalculateIdleTimeSecondsAsync(Place place, MissionType type, DateTime startedAtUtc, CancellationToken cancellationToken)
    {
        long? idleSeconds = 0;

        if (type == MissionType.PAYMENT || type == MissionType.PAYMENT_EC)
        {
            var lastOrderFinished = await _dbContext.Missions
                .Where(m => m.PlaceId == place.Id && m.Type == MissionType.ORDER && m.Status == MissionStatus.FINISHED && m.FinishedAt != null)
                .OrderByDescending(m => m.FinishedAt)
                .Select(m => m.FinishedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastOrderFinished.HasValue)
            {
                idleSeconds = (long)(startedAtUtc - lastOrderFinished.Value).TotalSeconds;
            }

            return idleSeconds < 0 ? 0 : idleSeconds;
        }

        var previousMissionFinished = await _dbContext.Missions
            .Where(m => m.PlaceId == place.Id && m.Type == type && m.Status == MissionStatus.FINISHED && m.FinishedAt != null)
            .OrderByDescending(m => m.FinishedAt)
            .Select(m => m.FinishedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (!previousMissionFinished.HasValue)
        {
            return null;
        }

        idleSeconds = (long)(startedAtUtc - previousMissionFinished.Value).TotalSeconds;

        var lastPayment = await _dbContext.Missions
            .Where(m => m.PlaceId == place.Id
                        && (m.Type == MissionType.PAYMENT || m.Type == MissionType.PAYMENT_EC)
                        && m.Status == MissionStatus.FINISHED
                        && m.StartedAt != default)
            .OrderByDescending(m => m.StartedAt)
            .Select(m => new { m.StartedAt, m.FinishedAt })
            .FirstOrDefaultAsync(cancellationToken);

        if (lastPayment != null && lastPayment.StartedAt > previousMissionFinished.Value)
        {
            idleSeconds = 0;
        }

        return idleSeconds < 0 ? 0 : idleSeconds;
    }

    private async Task StopMissionAsync(
        Mission mission,
        MissionStatus targetStatus,
        DateTime? when,
        long? userId,
        bool saveImmediately,
        CancellationToken cancellationToken)
    {
        var timestamp = when?.ToUniversalTime() ?? DateTime.UtcNow;

        if (mission.Status != MissionStatus.STARTED && mission.Status != MissionStatus.ACKNOWLEDGED)
        {
            _logger.LogDebug("Mission {MissionId} already finished with status {Status}", mission.Id, mission.Status);
            return;
        }

        mission.Status = targetStatus;
        mission.FinishedAt ??= timestamp;
        var finishedAtValue = mission.FinishedAt.Value;

        if (userId.HasValue)
        {
            mission.AssignedUserId = userId;
        }

        mission.MissionDurationSeconds = (long)Math.Max(0, (finishedAtValue - mission.StartedAt).TotalSeconds);

        if (saveImmediately)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task HandleServeTrackingAsync(Mission mission, DateTime finishedAtUtc, CancellationToken cancellationToken)
    {
        if (!_options.TrackServeMission)
        {
            return;
        }

        if (mission.Type != MissionType.ORDER || !mission.PlaceId.HasValue)
        {
            return;
        }

        var result = await StartMissionAsync(
            mission.PlaceId,
            MissionType.SERVE,
            mission.AssignedUserId ?? mission.Place?.UserId,
            finishedAtUtc,
            cancellationToken: cancellationToken);

        if (result.CreatedNew)
        {
            _logger.LogInformation("Started SERVE mission {MissionId} after finishing ORDER {OrderMissionId}", result.Mission.Id, mission.Id);
        }
    }
}

#pragma warning restore CS8602
