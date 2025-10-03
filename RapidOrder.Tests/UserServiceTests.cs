using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Infrastructure;
using RapidOrder.Infrastructure.Services;
using RapidOrder.Core.Services;
using RapidOrder.Core.Enums;
using Xunit;

namespace RapidOrder.Tests.Services;

public class UserServiceTests
{
    private static RapidOrderDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RapidOrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RapidOrderDbContext(options);
    }

    [Fact]
    public async Task CreateAndRetrieveUser_Works()
    {
        await using var context = CreateContext();
        IUserService service = new UserService(context);

        var user = new User
        {
            Login = "waiter@example.com",
            Password = new string('x', 60),
            Role = UserRole.WAITER
        };

        var created = await service.CreateAsync(user);

        Assert.NotEqual(0, created.Id);

        var fromDb = await service.GetByIdAsync(created.Id);

        Assert.NotNull(fromDb);
        Assert.Equal("waiter@example.com", fromDb!.Login);
    }

    [Fact]
    public async Task AssignMission_PersistsMissionAndLink()
    {
        await using var context = CreateContext();
        IUserService service = new UserService(context);

        var user = await service.CreateAsync(new User
        {
            Login = "staff@example.com",
            Password = new string('y', 60)
        });

        var mission = new Mission
        {
            Type = MissionType.SERVICE,
            Status = MissionStatus.STARTED,
            StartedAt = DateTime.UtcNow
        };

        await service.AssignMissionAsync(user.Id, mission);

        var updated = await service.GetByIdAsync(user.Id);

        Assert.NotNull(updated);
        Assert.Single(updated!.Missions);
        Assert.Equal(user.Id, updated.Missions.First().AssignedUserId);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        await using var context = CreateContext();
        IUserService service = new UserService(context);

        var user = await service.CreateAsync(new User
        {
            Login = "delete@example.com",
            Password = new string('z', 60)
        });

        await service.DeleteAsync(user.Id);

        Assert.Null(await service.GetByIdAsync(user.Id));
    }
}
