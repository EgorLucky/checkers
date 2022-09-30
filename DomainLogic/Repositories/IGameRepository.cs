using DomainLogic.Filters;
using DomainLogic.Models;
using System;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IGameRepository
    {
        Task Create(Game game);
        Task<Game> Get(GameGetFilter filter);
        Task Update(Game game);
    }
}