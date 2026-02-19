using DomainLogic.Services;
using DomainLogic.ResultModels;
using DomainLogic.Models;

namespace Implementations.GameServiceHttpClient;

public class GameServiceHttpClient(IGameServiceRefitHttpClient client) : IGameServiceClient
{
    public Task<GameGetInfoResult> GetInfo(Guid gameId) => client.GetInfo(gameId);

    public Task<MoveResult> MakeMove(MoveVector move, Guid playerCode, Guid previousBoardStateId) => 
        client.MakeMove(move, playerCode, previousBoardStateId);
    public Task<GameRegisterSecondPlayerResult> RegisterSecondPlayer(Guid gameId) => 
        client.RegisterSecondPlayer(gameId);

    public async Task ReadyToPlay(Guid playerCode) => await client.ReadyToPlay(playerCode);
}