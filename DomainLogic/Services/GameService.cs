using DomainLogic.Models;
using DomainLogic.Repositories;
using DomainLogic.ResultModels;
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

        public async Task<GameCreateResult> Create()
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                CreateDateTime = DateTimeOffset.Now,
                FirstPlayerCode = Guid.NewGuid(),
                State = GameState.Created
                //SecondPlayerCode = Guid.NewGuid()
            };

            await _repository.Create(game);

            return new GameCreateResult(game.Id, game.FirstPlayerCode.Value);
        }

        public async Task<GameGetRegistrationStatusResult> GetRegistrationStatus(Guid gameId)
        {
            var game = await _repository.Get(new Filters.GameGetFilter(Id: gameId));

            if (game == null)
                return new GameGetRegistrationStatusResult(
                    Success: false, 
                    Message: "game not found");

            return new GameGetRegistrationStatusResult(
                SecondUserRegistred: game.SecondPlayerCode != null);
        }

        public async Task<GameRegisterSecondUserResult> RegisterSecondUser(Guid gameId)
        {
            var game = await _repository.Get(new Filters.GameGetFilter(Id: gameId));

            if (game == null)
                return new GameRegisterSecondUserResult(
                    Success: false,
                    Message: "game not found");

            if (game.SecondPlayerCode != null)
                return new GameRegisterSecondUserResult(
                    Success: false,
                    Message: "second user already registred");

            game.SecondPlayerCode = Guid.NewGuid();

            await _repository.Update(game);

            return new GameRegisterSecondUserResult(Code: game.SecondPlayerCode);
        }

        public async Task<GameStartResult> StartWithBot(Guid firstPlayerCode)
        {
            var startResult = await Start(firstPlayerCode);

            if(startResult.Success && startResult.AwaitableMove == AwaitableMove.SecondPlayer)
            {
                //notify bot
            }

            return startResult;
        }

        public async Task<GameStartResult> Start(Guid firstPlayerCode)
        {
            var game = await _repository.Get(new Filters.GameGetFilter(FirstPlayerCode: firstPlayerCode));

            if (game == null)
                return new GameStartResult(
                    Success: false,
                    Message: "game not found");

            if (game.SecondPlayerCode == null)
                return new GameStartResult(
                    Success: false,
                    Message: "second gamer didn't join");

            if ((new Random()).Next(2) % 2 == 0)
            {
                //opponent move
                game.AwaitableMove = AwaitableMove.SecondPlayer;
            }
            else
            {
                game.AwaitableMove = AwaitableMove.FirstPlayer;
            }

            game.State = GameState.Running;

            await _repository.Update(game);

            return new GameStartResult(AwaitableMove: game.AwaitableMove);
        }

    }
}
