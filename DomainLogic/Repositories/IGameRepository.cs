using DomainLogic.Models;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IGameRepository
    {
        Task Create(Game game);
    }
}