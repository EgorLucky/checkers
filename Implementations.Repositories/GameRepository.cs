using DomainLogic.Filters;
using DomainLogic.Models;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Game> Get(GameGetFilter filter)
        {
            if(filter.Id != null)
                return Games.Where(g => g.Id == filter.Id).FirstOrDefault();

            return Games.Where(g => g.FirstPlayerCode == filter.FirstPlayerCode).FirstOrDefault();
        }

        public async Task Update(Game game)
        {
            var index = Games.FindIndex(g => g.Id == game.Id);
            if (index == -1)
                throw new Exception("game not found");

            Games[index] = game;
        }
    }
}
