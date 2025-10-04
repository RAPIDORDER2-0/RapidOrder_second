using System.Net.Http.Json;
using RapidOrder.Core.Entities;
using Xunit;

namespace RapidOrder.Tests.Integration;

public class PlacesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PlacesApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPlaces_ReturnsEmptyList_ByDefault()
    {
        var response = await _client.GetAsync("/api/places");
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Expected success but got {(int)response.StatusCode}: {body}");

        var payload = await response.Content.ReadFromJsonAsync<List<Place>>();
        Assert.NotNull(payload);
        Assert.Empty(payload!);
    }

}
