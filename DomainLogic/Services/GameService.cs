using DomainLogic.Models;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class GameService
    {
        public readonly IGameRepository _repository;

        public GameService(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<Game> Create()
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                CreateDateTime = DateTimeOffset.Now
            };

            await _repository.Create(game);

            return game;
        }
    }
}
