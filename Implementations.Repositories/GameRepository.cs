using DomainLogic.Models;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Implementations.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly static List<Game> Games = new List<Game>();

        public async Task Create(Game game)
        {
            Games.Add(game);
        }
    }
}
