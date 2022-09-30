using AutoMapper;
using DomainLogic.Models;
using DomainLogic.Repositories;
using Implementations.RepositoriesEF.Entitites;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using DomainPlayerGameData = DomainLogic.Models.PlayerGameData;
using PlayerGameData = Implementations.RepositoriesEF.Entitites.PlayerGameData;

namespace Implementations.RepositoriesEF
{
    public class BotRepository : IBotRepository
    {
        private readonly GameDbContext _context;
        private readonly IMapper _mapper;

        public BotRepository(GameDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DomainPlayerGameData> Get(Guid gameId)
        {
            var dbData = await _context.BotPlayerGameData.FirstOrDefaultAsync(b => b.GameId == gameId);
            var result = _mapper.Map<DomainPlayerGameData>(dbData);
            return result;
        }

        public Task RemoveGameData(Guid gameId)
        {
            return _context.BotPlayerGameData.Where(b => b.GameId == gameId).DeleteAsync();
        }

        public async Task SavePlayerGameData(DomainPlayerGameData data)
        {
            var dbData = _mapper.Map<PlayerGameData>(data);
            await _context.AddAsync(dbData);
            await _context.SaveChangesAsync();
        }
    }
}