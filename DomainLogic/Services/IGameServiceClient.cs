using DomainLogic.Models;
using DomainLogic.ResultModels;
using System;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public interface IGameServiceClient
    {
        Task<GameRegisterSecondPlayerResult> RegisterSecondPlayer(Guid gameId);
        Task<GameGetInfoResult> GetInfo(Guid gameId);
        Task<object> MakeMove(MoveVector move, Guid playerCode);
    }
}