using System.Net.Http.Json;
using System.Text.Json;
using DomainLogic.Models;
using DomainLogic.Services;
using Game = Implementations.RepositoriesEF.Entitites.Game;

namespace Tests;

public class GameGetInfoTest(AspireCustomWebApplicationFactory factory)
    : IClassFixture<AspireCustomWebApplicationFactory>
{
    [Fact]
    public async Task Handle_GetGameInfo_With_GameCreated_Status()
    {
        var client = factory.CreateClient();
        var dbContext = await factory.GetDbContext();
        var game = new Game
        {
            Id = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            State = GameState.Created,
            FirstPlayerCheckerColor = "#fsdfs",
            OpponentCheckerColor = "#fsdfs",
            CheckerCellColor = "#fsdfs",
            NonPlayableCellColor = "#fsdfs",
            FirstPlayerBoardSide = BoardSide.FirstSide,
            SecondPlayerBoardSide = BoardSide.SecondSide,
            Players = [
                new() {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.FirstPlayer
                },
                new () {
                    Type = GamePlayer.SecondPlayer
                }
            ]
        };
        
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("/game/getInfo?gameId=" + game.Id);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            
        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            
        Assert.NotNull(json);
        var root = json.RootElement;
        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal(game.State.ToString(), root.GetProperty("state").GetString(), ignoreCase: true);
    }
    
    [Fact]
    public async Task Handle_GetGameInfo_With_AllPlayersRegistred_Status()
    {
        var client = factory.CreateClient();
        var dbContext = await factory.GetDbContext();
        var game = new Game
        {
            Id = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            State = GameState.AllPlayersRegistred,
            FirstPlayerCheckerColor = "#fsdfs",
            OpponentCheckerColor = "#fsdfs",
            CheckerCellColor = "#fsdfs",
            NonPlayableCellColor = "#fsdfs",
            FirstPlayerBoardSide = BoardSide.FirstSide,
            SecondPlayerBoardSide = BoardSide.SecondSide,
            Players = [
                new() {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.FirstPlayer
                },
                new () {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.SecondPlayer
                }
            ]
        };
        
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("/game/getInfo?gameId=" + game.Id);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            
        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            
        Assert.NotNull(json);
        var root = json.RootElement;
        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal(game.State.ToString(), root.GetProperty("state").GetString(), ignoreCase: true);
    }
    
    [Fact]
    public async Task Handle_GetGameInfo_With_SecondPlayerReadyToPlay_Status()
    {
        var client = factory.CreateClient();
        var dbContext = await factory.GetDbContext();
        var game = new Game
        {
            Id = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            State = GameState.SecondPlayerReadyToPlay,
            FirstPlayerCheckerColor = "#fsdfs",
            OpponentCheckerColor = "#fsdfs",
            CheckerCellColor = "#fsdfs",
            NonPlayableCellColor = "#fsdfs",
            FirstPlayerBoardSide = BoardSide.FirstSide,
            SecondPlayerBoardSide = BoardSide.SecondSide,
            Players = [
                new() {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.FirstPlayer
                },
                new () {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.SecondPlayer
                }
            ]
        };
        
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("/game/getInfo?gameId=" + game.Id);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            
        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            
        Assert.NotNull(json);
        var root = json.RootElement;
        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal(game.State.ToString(), root.GetProperty("state").GetString(), ignoreCase: true);
    }
    
    [Fact]
    public async Task Handle_GetGameInfo_With_Running_Status()
    {
        var client = factory.CreateClient();
        var dbContext = await factory.GetDbContext();
        var mongoDbContext = await factory.GetMongoDbContext();
        var game = new Game
        {
            Id = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            StartDateTime =  DateTimeOffset.UtcNow,
            State = GameState.Running,
            FirstPlayerCheckerColor = "#fsdfs",
            OpponentCheckerColor = "#fsdfs",
            CheckerCellColor = "#fsdfs",
            NonPlayableCellColor = "#fsdfs",
            FirstPlayerBoardSide = BoardSide.FirstSide,
            SecondPlayerBoardSide = BoardSide.SecondSide,
            Players = [
                new() {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.FirstPlayer
                },
                new () {
                    Id = Guid.NewGuid(),
                    Type = GamePlayer.SecondPlayer
                }
            ]
        };
        
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        var domainGame = new DomainLogic.Models.Game();
        domainGame.Id = game.Id;
        domainGame.CheckerCellColor = game.CheckerCellColor;
        domainGame.NonPlayableCellColor = game.NonPlayableCellColor;
        domainGame.FirstPlayerBoardSide = game.FirstPlayerBoardSide;
        domainGame.SecondPlayerBoardSide = game.SecondPlayerBoardSide;
        domainGame.FirstPlayerCheckerColor = game.FirstPlayerCheckerColor;
        domainGame.OpponentCheckerColor = game.OpponentCheckerColor;
        
        var moveManager = new MoveManager();
        var boardState = await moveManager.InitializeHistory(domainGame);
        await mongoDbContext.BoardStates.InsertOneAsync(boardState);
        
        var response = await client.GetAsync("/game/getInfo?gameId=" + game.Id);

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            
        var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            
        Assert.NotNull(json);
        var root = json.RootElement;
        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.Equal(game.State.ToString(), root.GetProperty("state").GetString(), ignoreCase: true);
        Assert.Equal(JsonValueKind.Object, root.GetProperty("boardState").ValueKind);
    }
}