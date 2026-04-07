using System.Net.Http.Json;
using System.Text.Json;

namespace Tests;

public class GameCreateTest(AspireCustomWebApplicationFactory factory)
    : IClassFixture<AspireCustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_GameCreateTest()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/game/create", new
        {
            myCheckerColor = "#000000",
            opponentCheckerColor = "#FFFFFF",
            checkerCellColor = "#000000",
            nonPlayableCellColor = "#FFFFFF",
            myBoardSide = "firstSide"
        });

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            
        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            
        Assert.NotNull(json);
        Assert.True(json.RootElement.TryGetProperty("id", out var id));
        Assert.True(json.RootElement.TryGetProperty("firstPlayerCode", out var firstPlayerCode));
        Assert.True(Guid.TryParse(firstPlayerCode.ToString(), out var firstPlayerCodeGuid));
        Assert.True(Guid.TryParse(id.ToString(), out var idGuid));
        Assert.NotEqual(Guid.Empty, idGuid);
        Assert.NotEqual(Guid.Empty, firstPlayerCodeGuid);
    }
}