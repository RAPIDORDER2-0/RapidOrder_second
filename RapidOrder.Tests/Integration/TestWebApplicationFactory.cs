using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Core.Services;
using RapidOrder.Infrastructure;
using RapidOrder.Tests.Integration.Authentication;

namespace RapidOrder.Tests.Integration;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["UseInMemoryDatabase"] = "true",
                ["InMemoryDatabaseName"] = $"RapidOrderApiTests-{Guid.NewGuid()}"
            };
            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { });

            services.RemoveAll(typeof(ISettingsService));
            services.RemoveAll(typeof(IMissionService));
            services.RemoveAll(typeof(IHostedService));

            services.AddSingleton<ISettingsService, FakeSettingsService>();
            services.AddSingleton<IMissionService, FakeMissionService>();
        });
    }
}

internal sealed class FakeSettingsService : ISettingsService
{
    public bool TrackServedMission { get; set; }

    public Task<bool> GetTrackServedMissionAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(TrackServedMission);

    public Task SetTrackServedMissionAsync(bool value, CancellationToken cancellationToken = default)
    {
        TrackServedMission = value;
        return Task.CompletedTask;
    }
}

internal sealed class FakeMissionService : IMissionService
{
    public Task<MissionStartResult> StartMissionAsync(int? placeId, MissionType type, long? userId = null, DateTime? startedAt = null, string? sourceDecoded = null, int? sourceButton = null, CancellationToken cancellationToken = default)
    {
        var mission = new Mission
        {
            Id = 1,
            PlaceId = placeId,
            Type = type,
            Status = MissionStatus.STARTED,
            StartedAt = startedAt ?? DateTime.UtcNow
        };
        return Task.FromResult(new MissionStartResult(mission, true));
    }

    public Task<Mission?> FinishMissionAsync(long missionId, long? userId = null, DateTime? finishedAt = null, CancellationToken cancellationToken = default) => Task.FromResult<Mission?>(null);
    public Task<IReadOnlyList<Mission>> FinishPlaceMissionsAsync(int placeId, long? userId = null, DateTime? finishedAt = null, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Mission>>(Array.Empty<Mission>());
    public Task<Mission?> CancelMissionAsync(long missionId, DateTime? canceledAt = null, CancellationToken cancellationToken = default) => Task.FromResult<Mission?>(null);
    public Task<int> CancelPlaceMissionsAsync(int placeId, DateTime? canceledAt = null, CancellationToken cancellationToken = default) => Task.FromResult(0);
    public Task<int> CancelAllOpenMissionsAsync(DateTime? canceledAt = null, CancellationToken cancellationToken = default) => Task.FromResult(0);
    public Task<IReadOnlyList<Mission>> GetStartedMissionsByPlaceAsync(int placeId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Mission>>(Array.Empty<Mission>());
}
