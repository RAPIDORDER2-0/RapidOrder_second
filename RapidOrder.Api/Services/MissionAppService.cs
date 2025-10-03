using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.DTOs;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Core.Services;
using RapidOrder.Infrastructure;

namespace RapidOrder.Api.Services
{
    public class MissionAppService
    {
        private readonly RapidOrderDbContext _db;
        private readonly MissionNotifier _notifier;
        private readonly LearningModeService _learningModeService;
        private readonly IMissionService _missionService;

        public MissionAppService(
            RapidOrderDbContext db,
            MissionNotifier notifier,
            LearningModeService learningModeService,
            IMissionService missionService)
        {
            _db = db;
            _notifier = notifier;
            _learningModeService = learningModeService;
            _missionService = missionService;
        }

        // Called by the file watcher when it decodes a signal
        public async Task<long> CreateMissionFromSignalAsync(string decoded, int button, DateTime ts, CancellationToken ct = default)
        {
            // 1) Find CallButton by HEX device code
            var callButton = await _db.CallButtons
                .Include(cb => cb.Place)
                .FirstOrDefaultAsync(cb => cb.DeviceCode == decoded, ct);

            if (callButton == null)
            {
                if (_learningModeService.IsLearningModeEnabled)
                {
                    var newCallButton = new CallButton
                    {
                        DeviceCode = decoded,
                        ButtonId = decoded, // Or some other default
                        Label = $"New Button {decoded}",
                        PlaceId = null
                    };
                    _db.CallButtons.Add(newCallButton);
                    await _db.SaveChangesAsync(ct);
                    // Optionally, log that a new button was learned
                    _db.EventLogs.Add(new EventLog
                    {
                        Type = EventType.System,
                        CreatedAt = ts,
                        PayloadJson = $"{{\"learnedCallButton\":\"{decoded}\",\"button\":{button}}}"
                    });
                    await _db.SaveChangesAsync(ct);
                    return 0; // Don't create a mission for the learning signal
                }

                // Unknown device: log and bail (no Mission)
                _db.EventLogs.Add(new EventLog
                {
                    Type = EventType.MissionCreated, // keeping enum; payload explains it’s unknown
                    CreatedAt = ts,
                    PayloadJson = $"{{\"unknownCallButton\":\"{decoded}\",\"button\":{button}}}"
                });
                await _db.SaveChangesAsync(ct);
                return 0;
            }

            // 2) Resolve MissionType from per-button mapping
            var missionType = button switch
            {
                1 => MissionType.ORDER,
                2 => MissionType.PAYMENT,
                3 => MissionType.PAYMENT_EC,
                4 => MissionType.SERVE,
                _ => MissionType.ASSISTANCE // fallback/default
            };

            // 3) Create Mission for the mapped Place
            var result = await _missionService.StartMissionAsync(
                callButton.PlaceId,
                missionType,
                callButton.Place?.UserId,
                ts,
                decoded,
                button,
                ct);

            if (!result.CreatedNew)
            {
                return result.Mission.Id;
            }

            var mission = result.Mission;

            // 4) EventLog
            _db.EventLogs.Add(new EventLog
            {
                Type = EventType.MissionCreated,
                CreatedAt = ts,
                PlaceId = callButton.PlaceId,
                PayloadJson = $"{{\"device\":\"{decoded}\",\"button\":{button}}}"
            });

            await _db.SaveChangesAsync(ct);

            // 5) Notify SignalR
            var placeLabel = callButton.Place != null
                ? $"{callButton.Place.Description} (#{callButton.Place.Number})"
                : "Unassigned";

            var dto = new MissionCreatedDto
            {
                Id = mission.Id,
                Type = mission.Type,
                Status = mission.Status,
                StartedAt = mission.StartedAt,
                PlaceId = callButton.PlaceId,
                PlaceLabel = placeLabel,
                SourceDecoded = mission.SourceDecoded,
                SourceButton = mission.SourceButton
            };
            await _notifier.PushCreatedAsync(dto);

            return mission.Id;
        }

        public async Task<Mission?> UpdateMissionAsync(long id, MissionStatus status, long? userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            if (status == MissionStatus.ACKNOWLEDGED)
            {
                var mission = await _db.Missions.Include(x => x.Place).FirstOrDefaultAsync(x => x.Id == id, ct);
                if (mission == null)
                {
                    return null;
                }

                mission.Status = status;
                mission.AcknowledgedAt ??= now;
                if (userId.HasValue)
                {
                    mission.AssignedUserId = userId;
                }

                if (mission.AcknowledgedAt.HasValue)
                {
                    mission.IdleTimeSeconds = (long)(mission.AcknowledgedAt.Value - mission.StartedAt).TotalSeconds;
                }

                _db.EventLogs.Add(new EventLog { Type = MapEvent(status), CreatedAt = now, MissionId = mission.Id, PlaceId = mission.PlaceId, UserId = userId });
                await _db.SaveChangesAsync(ct);

                await PushUpdateAsync(mission);
                return mission;
            }

            if (status == MissionStatus.FINISHED)
            {
                var mission = await _missionService.FinishMissionAsync(id, userId, now, ct);
                if (mission == null)
                {
                    return null;
                }

                _db.EventLogs.Add(new EventLog { Type = MapEvent(status), CreatedAt = mission.FinishedAt ?? now, MissionId = mission.Id, PlaceId = mission.PlaceId, UserId = userId });
                await _db.SaveChangesAsync(ct);

                await PushUpdateAsync(mission);
                return mission;
            }

            if (status == MissionStatus.CANCELED)
            {
                var mission = await _missionService.CancelMissionAsync(id, now, ct);
                if (mission == null)
                {
                    return null;
                }

                _db.EventLogs.Add(new EventLog { Type = MapEvent(status), CreatedAt = mission.FinishedAt ?? now, MissionId = mission.Id, PlaceId = mission.PlaceId, UserId = userId });
                await _db.SaveChangesAsync(ct);

                await PushUpdateAsync(mission);
                return mission;
            }

            return await _db.Missions.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        private static EventType MapEvent(MissionStatus s) => s switch
        {
            MissionStatus.ACKNOWLEDGED => EventType.MissionAcknowledged,
            MissionStatus.FINISHED => EventType.MissionFinished,
            MissionStatus.CANCELED => EventType.MissionCanceled,
            _ => EventType.MissionCreated
        };

        private async Task PushUpdateAsync(Mission mission)
        {
            var placeLabelForUpdate = mission.Place != null
                ? $"{mission.Place.Description} (#{mission.Place.Number})"
                : "Unassigned";

            var dto = new MissionCreatedDto
            {
                Id = mission.Id,
                Type = mission.Type,
                Status = mission.Status,
                StartedAt = mission.StartedAt,
                PlaceId = mission.PlaceId,
                PlaceLabel = placeLabelForUpdate,
                SourceDecoded = mission.SourceDecoded,
                SourceButton = mission.SourceButton
            };

            await _notifier.PushUpdatedAsync(dto);
        }
    }
}
