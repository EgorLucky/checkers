using DomainLogic.Filters;
using DomainLogic.Models;
using DomainLogic.Repositories;
using MongoDB.Driver;

namespace Implementations.RepositoriesMongoDB
{
    public class MongoGameRepository : IGameRepository
    {
        private readonly GameMongoDBContext _context;

        public MongoGameRepository(GameMongoDBContext context)
        {
            _context = context;
        }

        public async Task Create(Game game)
        {
            await _context.Games.InsertOneAsync(game);
        }

        public async Task<Game> Get(GameGetFilter filter)
        {
            if (filter.Id != null)
                return await _context
                    .Games
                    .Find(g => g.Id == filter.Id)
                    .FirstOrDefaultAsync();
            if (filter.FirstPlayerCode != null)
                return await _context
                    .Games
                    .Find(g => g.FirstPlayerCode == filter.FirstPlayerCode)
                    .FirstOrDefaultAsync();
            if (filter.PlayerCode != null)
                return await _context
                    .Games
                    .Find(g => g.FirstPlayerCode == filter.PlayerCode
                        || g.SecondPlayerCode == filter.PlayerCode)
                    .FirstOrDefaultAsync();
            throw new ArgumentException("all filter properties are empty");
        }

        public async Task Update(Game game)
        {
            await _context.Games.ReplaceOneAsync(g => g.Id == game.Id, game);
        }
    }
}