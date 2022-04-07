using DomainLogic.Models;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implementations.Repositories
{
    public class GameHistoryRepository : IGameHistoryRepository
    {
        private static List<BoardState> boardStates = new List<BoardState>();

        public async Task<BoardState> GetLastBoardState(Guid gameId)
        {
            return boardStates
                .Where(b => b.GameId == gameId)
                .OrderByDescending(b => b.CreateDateTime)
                .FirstOrDefault();
        }

        public async Task SaveBoardState(BoardState boardState)
        {
            boardStates.Add(boardState);
        }
    }
}
