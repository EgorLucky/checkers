using DomainLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IBotRepository
    {
        Task SavePlayerGameData(PlayerGameData data);

        Task<PlayerGameData> Get(Guid gameId);

        Task RemoveGameData(Guid gameId);
    }
}
