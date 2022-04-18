using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class Bot
    {
        private readonly IGameServiceClient _service;
        private readonly IBotRepository _botRepository;

        public Bot(IGameServiceClient service, IBotRepository botRepository)
        {
            _service = service;
            _botRepository = botRepository;
        }

        public async Task Register(Guid gameId)
        {
            var registerResult = await _service.RegisterSecondPlayer(gameId);

            if (registerResult.Success)
            {
                var gameCode = registerResult.Code.Value;
                await _botRepository.SavePlayerGameData(new Models.PlayerGameData(
                    gameId,
                    gameCode));
            }
            else
            {
                //think what to do
            }
        }

        public Task Move(Guid gameId)
        {
            throw new NotImplementedException();
        }
    }
}
