using DomainLogic.Models;
using DomainLogic.ResultModels;
using Refit;

namespace Implementations.GameServiceHttpClient;

public interface IGameServiceRefitHttpClient
{
    [Get("/Game/getInfo")]
    Task<GameGetInfoResult> GetInfo([Query] Guid gameId);

    [Post("/Game/move")]
    Task<MoveResult> MakeMove(
        [Body(BodySerializationMethod.Serialized)]
        MoveVector move,
        [Header("playerCode")]
        Guid playerCode,
        [Header("previousBoardStateId")]
        Guid previousBoardStateId);

    [Post("/Game/registerSecondPlayer")]
    Task<GameRegisterSecondPlayerResult> RegisterSecondPlayer(
        [Body(BodySerializationMethod.Serialized)]
        Guid gameId);

    [Post("/Game/readyToPlay")]
    Task ReadyToPlay([Header("playerCode")] Guid playerCode);
}