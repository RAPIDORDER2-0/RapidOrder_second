using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using RapidOrder.Core.Services;
using Xunit;

namespace RapidOrder.Tests.Integration;

public class SettingsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ISettingsService _settingsService;

    public SettingsApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _settingsService = factory.Services.GetRequiredService<ISettingsService>();
    }

    [Fact]
    public async Task GetTrackServedMission_ReturnsCurrentValue()
    {
        if (_settingsService is FakeSettingsService fake)
        {
            fake.TrackServedMission = false;
        }

        var response = await _client.GetAsync("/api/settings/track-served-mission");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TrackServedMissionResponse>();

        Assert.NotNull(payload);
        Assert.False(payload!.TrackServedMission);
    }

    [Fact]
    public async Task SetTrackServedMission_UpdatesValue()
    {
        var updateResponse = await _client.PutAsJsonAsync(
            "/api/settings/track-served-mission",
            new TrackServedMissionRequest(true));

        Assert.Equal(System.Net.HttpStatusCode.NoContent, updateResponse.StatusCode);

        var fetchResponse = await _client.GetAsync("/api/settings/track-served-mission");
        fetchResponse.EnsureSuccessStatusCode();

        var payload = await fetchResponse.Content.ReadFromJsonAsync<TrackServedMissionResponse>();

        Assert.NotNull(payload);
        Assert.True(payload!.TrackServedMission);
    }

    private sealed record TrackServedMissionResponse(bool TrackServedMission);

    private sealed record TrackServedMissionRequest(bool TrackServedMission);
}
