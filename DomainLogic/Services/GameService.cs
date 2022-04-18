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
        private readonly IGameRepository _repository;
        private readonly IGameHistoryRepository _gameHistoryRepository;
        private readonly IBotNotifier _botNotifier;
        private readonly MoveManager _moveManager;

        public GameService(
            IGameRepository repository,
            IGameHistoryRepository gameHistoryRepository,
            IBotNotifier botNotifier,
            MoveManager moveManager)
        {
            _repository = repository;
            _botNotifier = botNotifier;
            _moveManager = moveManager;
            _gameHistoryRepository = gameHistoryRepository;
        }

        public async Task<GameCreateResult> Create()
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                CreateDateTime = DateTimeOffset.Now,
                FirstPlayerCode = Guid.NewGuid(),
                State = GameState.Created
            };

            await _repository.Create(game);

            return new GameCreateResult(game.Id, game.FirstPlayerCode.Value);
        }

        public async Task<GameCreateResult> CreateWithBot()
        {
            var createResult = await Create();
            await _botNotifier.RegisterNotify(createResult.Id);
            return createResult;
        }

        public async Task<GameGetInfoResult> GetInfo(Guid gameId)
        {
            var game = await _repository.Get(new Filters.GameGetFilter(Id: gameId));

            if (game == null)
                return new GameGetInfoResult(
                    Success: false, 
                    Message: "game not found");

            var boardState = await _gameHistoryRepository.GetLastBoardState(gameId);

            return new GameGetInfoResult(
                State: game.State,
                AwaitableMove: game.AwaitableMove,
                BoardState: boardState);
        }

        public async Task<GameRegisterSecondPlayerResult> RegisterSecondPlayer(Guid gameId)
        {
            var game = await _repository.Get(new Filters.GameGetFilter(Id: gameId));

            if (game == null)
                return new GameRegisterSecondPlayerResult(
                    Success: false,
                    Message: "game not found");

            if (game.SecondPlayerCode != null)
                return new GameRegisterSecondPlayerResult(
                    Success: false,
                    Message: "second user already registred");

            game.SecondPlayerCode = Guid.NewGuid();
            game.State = GameState.AllPlayersRegistred;

            await _repository.Update(game);

            return new GameRegisterSecondPlayerResult(Code: game.SecondPlayerCode);
        }

        public async Task<GameStartResult> StartWithBot(Guid firstPlayerCode)
        {
            var startResult = await Start(firstPlayerCode);

            if(startResult.Success && startResult.AwaitableMove == AwaitableMove.SecondPlayer)
            {
                //notify bot
                await _botNotifier.MoveNotify(startResult.BoardState.GameId);
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

            var boardState = await _moveManager.InitializeHistory(game);

            await _gameHistoryRepository.SaveBoardState(boardState);

            return new GameStartResult(
                AwaitableMove: game.AwaitableMove,
                BoardState: boardState);
        }

    }
}
