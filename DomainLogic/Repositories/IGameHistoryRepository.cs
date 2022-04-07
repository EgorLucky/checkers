using DomainLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IGameHistoryRepository
    {
        Task SaveBoardState(BoardState boardState);
        Task<BoardState> GetLastBoardState(Guid gameId);
    }
}
