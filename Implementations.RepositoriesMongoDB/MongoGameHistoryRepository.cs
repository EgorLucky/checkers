using DomainLogic.Models;
using DomainLogic.Repositories;
using MongoDB.Driver;

namespace Implementations.RepositoriesMongoDB
{
    public class MongoGameHistoryRepository : IGameHistoryRepository
    {
        private readonly GameBoardStateMongoDBContext _context;

        public MongoGameHistoryRepository(GameBoardStateMongoDBContext context)
        {
            _context = context;
        }

        public async Task<BoardState> GetLastBoardState(Guid gameId)
        {
            return await _context
                .BoardStates
                .Find(x => x.GameId == gameId)
                .SortByDescending(d => d.CreateDateTime)
                .FirstOrDefaultAsync();
        }

        public Task SaveBoardState(BoardState boardState)
        {
            return _context.BoardStates.InsertOneAsync(boardState);
        }
    }
}