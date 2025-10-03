using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Core.Options;
using RapidOrder.Infrastructure;
using RapidOrder.Infrastructure.Services;
using RapidOrder.Core.Services;
using Xunit;

namespace RapidOrder.Tests.Services;

public class MissionServiceTests
{
    private static (MissionService Service, RapidOrderDbContext Context) CreateService(MissionServiceOptions? options = null)
    {
        var dbOptions = new DbContextOptionsBuilder<RapidOrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new RapidOrderDbContext(dbOptions);
        var logger = NullLogger<MissionService>.Instance;
        var opts = Options.Create(options ?? new MissionServiceOptions());
        var service = new MissionService(context, logger, opts);
        return (service, context);
    }

    private static Place SeedPlace(RapidOrderDbContext context)
    {
        var user = new User
        {
            Login = "waiter@example.com",
            Password = new string('x', 60),
            DisplayName = "Waiter"
        };
        context.Users.Add(user);

        var setup = new Setup { Name = "Default" };
        context.Setups.Add(setup);

        var group = new PlaceGroup { Name = "Main" };
        context.PlaceGroups.Add(group);

        var place = new Place
        {
            Number = 12,
            Description = "Table 12",
            Setup = setup,
            SetupId = setup.Id,
            PlaceGroup = group,
            PlaceGroupId = group.Id,
            User = user,
            UserId = user.Id
        };

        context.Places.Add(place);
        context.SaveChanges();

        return place;
    }

    [Fact]
    public async Task StartMission_CreatesMissionWithPlaceAndUser()
    {
        var (service, context) = CreateService();
        await using var _ = context;
        var place = SeedPlace(context);

        var result = await service.StartMissionAsync(place.Id, MissionType.ORDER, place.UserId);

        Assert.True(result.CreatedNew);
        var mission = result.Mission;
        Assert.Equal(MissionStatus.STARTED, mission.Status);
        Assert.Equal(place.Id, mission.PlaceId);
        Assert.Equal(place.PlaceGroupId, mission.PlaceGroupId);
        Assert.Equal(place.SetupId, mission.SetupId);
        Assert.Equal(place.UserId, mission.AssignedUserId);
    }

    [Fact]
    public async Task StartMission_PreventsDuplicateForSameUser()
    {
        var (service, context) = CreateService();
        await using var _ = context;
        var place = SeedPlace(context);

        var first = await service.StartMissionAsync(place.Id, MissionType.ORDER, place.UserId);
        var second = await service.StartMissionAsync(place.Id, MissionType.ORDER, place.UserId);

        Assert.True(first.CreatedNew);
        Assert.False(second.CreatedNew);
        Assert.Equal(first.Mission.Id, second.Mission.Id);
    }

    [Fact]
    public async Task StartMission_ComputesIdleTimeForPayment()
    {
        var (service, context) = CreateService();
        await using var _ = context;
        var place = SeedPlace(context);

        var previousOrder = new Mission
        {
            PlaceId = place.Id,
            Type = MissionType.ORDER,
            Status = MissionStatus.FINISHED,
            StartedAt = DateTime.UtcNow.AddMinutes(-20),
            FinishedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        context.Missions.Add(previousOrder);
        context.SaveChanges();

        var result = await service.StartMissionAsync(place.Id, MissionType.PAYMENT);

        Assert.True(result.CreatedNew);
        Assert.NotNull(result.Mission.IdleTimeSeconds);
        Assert.InRange(result.Mission.IdleTimeSeconds!.Value, 590, 610); // approx 10 minutes
    }

    [Fact]
    public async Task FinishMission_SetsDurationAndStartsServeWhenConfigured()
    {
        var (service, context) = CreateService(new MissionServiceOptions { TrackServeMission = true });
        await using var _ = context;
        var place = SeedPlace(context);

        var startedAt = DateTime.UtcNow.AddMinutes(-5);
        var result = await service.StartMissionAsync(place.Id, MissionType.ORDER, place.UserId, startedAt);

        var finished = await service.FinishMissionAsync(result.Mission.Id, place.UserId, DateTime.UtcNow);

        Assert.NotNull(finished);
        Assert.Equal(MissionStatus.FINISHED, finished!.Status);
        Assert.NotNull(finished.MissionDurationSeconds);
        Assert.True(finished.MissionDurationSeconds > 0);

        var serveMission = await context.Missions
            .Where(m => m.Type == MissionType.SERVE && m.PlaceId == place.Id)
            .OrderByDescending(m => m.StartedAt)
            .FirstOrDefaultAsync();

        Assert.NotNull(serveMission);
        Assert.Equal(MissionStatus.STARTED, serveMission!.Status);
    }

    [Fact]
    public async Task CancelPlaceMissions_MarksAllCancelled()
    {
        var (service, context) = CreateService();
        await using var _ = context;
        var place = SeedPlace(context);

        await service.StartMissionAsync(place.Id, MissionType.ORDER);
        await service.StartMissionAsync(place.Id, MissionType.PAYMENT);

        var cancelledCount = await service.CancelPlaceMissionsAsync(place.Id);

        Assert.Equal(2, cancelledCount);

        var statuses = await context.Missions.Where(m => m.PlaceId == place.Id).Select(m => m.Status).ToListAsync();
        Assert.All(statuses, status => Assert.Equal(MissionStatus.CANCELED, status));
    }
}
