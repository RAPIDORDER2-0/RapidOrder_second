using System;
using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Infrastructure;
using RapidOrder.Infrastructure.Services;
using RapidOrder.Core.Services;
using Xunit;

namespace RapidOrder.Tests.Services;

public class PlaceServiceTests
{
    private static RapidOrderDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RapidOrderDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RapidOrderDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_PersistsPlace()
    {
        await using var context = CreateContext();
        IPlaceService service = new PlaceService(context);

        var place = new Place
        {
            Number = 5,
            Description = "Main floor table"
        };

        var created = await service.CreateAsync(place);

        Assert.NotEqual(0, created.Id);

        var fromDb = await service.GetByIdAsync(created.Id);
        Assert.NotNull(fromDb);
        Assert.Equal(5, fromDb!.Number);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsMatchingPlaces()
    {
        await using var context = CreateContext();
        IPlaceService service = new PlaceService(context);

        var user = new User
        {
            Login = "owner@example.com",
            Password = new string('p', 60)
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await service.CreateAsync(new Place { Number = 1, UserId = user.Id });
        await service.CreateAsync(new Place { Number = 2, UserId = user.Id });
        await service.CreateAsync(new Place { Number = 3 });

        var places = await service.GetByUserAsync(user.Id);

        Assert.Equal(2, places.Count);
        Assert.All(places, p => Assert.Equal(user.Id, p.UserId));
    }

    [Fact]
    public async Task UpdateAsync_ModifiesEntity()
    {
        await using var context = CreateContext();
        IPlaceService service = new PlaceService(context);

        var place = await service.CreateAsync(new Place
        {
            Number = 9,
            Description = "Initial"
        });

        place.Description = "Updated";
        await service.UpdateAsync(place);

        var updated = await service.GetByIdAsync(place.Id);
        Assert.Equal("Updated", updated!.Description);
    }
}
