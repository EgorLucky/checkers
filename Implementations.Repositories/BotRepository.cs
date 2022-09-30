using DomainLogic.Models;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implementations.Repositories
{
    public class BotRepository : IBotRepository
    {
        private static List<PlayerGameData> PlayerData = new List<PlayerGameData>();
        public async Task<PlayerGameData> Get(Guid gameId)
        {
            return PlayerData.Where(p => p.GameId == gameId).FirstOrDefault();
        }

        public async Task RemoveGameData(Guid gameId)
        {
            PlayerData.RemoveAll(g => g.GameId == gameId);
        }

        public async Task SavePlayerGameData(PlayerGameData data)
        {
            PlayerData.Add(data);
        }
    }
}
