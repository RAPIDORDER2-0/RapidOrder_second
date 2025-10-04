using System;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Infrastructure;
using Xunit;

namespace RapidOrder.Tests.Integration;

public class NigrumApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public NigrumApiTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetEmployees_ReturnsSeededEmployee()
    {
        await SeedAsync();

        var response = await _client.GetAsync("/nigrum/employees");
        response.EnsureSuccessStatusCode();

        var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();

        Assert.NotNull(employees);
        Assert.Single(employees!);
        Assert.Equal("waiter@example.com", employees![0].DisplayName);
        Assert.Single(employees[0].Places);
    }

    private async Task SeedAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RapidOrderDbContext>();

        db.Users.RemoveRange(db.Users);
        db.Places.RemoveRange(db.Places);
        db.PlaceGroups.RemoveRange(db.PlaceGroups);
        db.Setups.RemoveRange(db.Setups);
        db.SaveChanges();

        var setup = new Setup { Name = "Default" };
        var group = new PlaceGroup { Name = "Terrasse", Setup = setup };
        var user = new User
        {
            Login = "waiter@example.com",
            Password = new string('x', 60),
            DisplayName = "waiter@example.com",
            Role = UserRole.WAITER
        };
        var place = new Place
        {
            Number = 1,
            Description = "Table 1",
            Setup = setup,
            PlaceGroup = group,
            User = user
        };

        db.Setups.Add(setup);
        db.PlaceGroups.Add(group);
        db.Users.Add(user);
        db.Places.Add(place);
        await db.SaveChangesAsync();
    }

    private sealed record EmployeeDto(long Id, string DisplayName, bool IsOnBreak, DateTime? BreakStartedAt, List<PlaceDto> Places);
    private sealed record PlaceDto(int Id, int Number, string Description, int? PlaceGroupId, string? PlaceGroupName, long? AssignedUserId);
}
