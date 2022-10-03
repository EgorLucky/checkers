using DomainLogic.Filters;
using DomainLogic.Models;
using DomainLogic.ParameterModels;
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

        public async Task<GameCreateResult> Create(GameCreateDTO dto)
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                CreateDateTime = DateTimeOffset.UtcNow,
                FirstPlayerCode = Guid.NewGuid(),
                State = GameState.Created,
                FirstPlayerCheckerColor = dto.MyCheckerColor,
                OpponentCheckerColor = dto.OpponentCheckerColor,
                CheckerCellColor = dto.CheckerCellColor,
                NonPlayableCellColor = dto.NonPlayableCellColor,
                FirstPlayerBoardSide = dto.MyBoardSide,
                SecondPlayerBoardSide = dto.OpponentBoardSide
            };

            await _repository.Create(game);

            return new GameCreateResult(game.Id, game.FirstPlayerCode.Value);
        }

        public async Task<GameCreateResult> CreateWithBot(GameCreateDTO dto)
        {
            var createResult = await Create(dto);
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
                BoardState: boardState,
                Winner: game.Winner);
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

        public async Task<SetReadyToPlayResult> SetReadyToPlay(Guid playerCode)
        {
            var game = await _repository.Get(new Filters.GameGetFilter(PlayerCode: playerCode));

            if (game is null || game.SecondPlayerCode.GetValueOrDefault() != playerCode)
                return new SetReadyToPlayResult(
                    Success: false,
                    Message: "game not found");

            if (game.SecondPlayerCode is null)
                return new SetReadyToPlayResult(
                    Success: false,
                    Message: "second gamer didn't join");

            game.State = GameState.SecondPlayerReadyToPlay;

            await _repository.Update(game);

            return new SetReadyToPlayResult(Success: true);
        }

        public async Task<GameStartResult> StartWithBot(Guid firstPlayerCode)
        {
            var startResult = await Start(firstPlayerCode);

            if(startResult.Success && startResult.AwaitableMove == GamePlayer.SecondPlayer)
            {
                //notify bot
                await _botNotifier.MoveNotify(startResult.BoardState.GameId);
            }

            return startResult;
        }

        public async Task<GameStartResult> Start(Guid firstPlayerCode)
        {
            var game = await _repository.Get(new GameGetFilter(FirstPlayerCode: firstPlayerCode));

            if (game == null)
                return new GameStartResult(
                    Success: false,
                    Message: "game not found");

            if (game.SecondPlayerCode == null)
                return new GameStartResult(
                    Success: false,
                    Message: "second gamer didn't join");

            if (game.State is not GameState.SecondPlayerReadyToPlay)
                return new GameStartResult(
                    Success: false,
                    Message: "second gamer doesn't ready to play");

            if ((new Random()).Next(2) % 2 == 0)
            {
                //opponent move
                game.AwaitableMove = GamePlayer.SecondPlayer;
            }
            else
            {
                game.AwaitableMove = GamePlayer.FirstPlayer;
            }

            game.State = GameState.Running;

            await _repository.Update(game);

            var boardState = await _moveManager.InitializeHistory(game);

            await _gameHistoryRepository.SaveBoardState(boardState);

            return new GameStartResult(
                AwaitableMove: game.AwaitableMove,
                BoardState: boardState);
        }

        public async Task<MoveResult> Move(Guid playerCode, Guid previousBoardStateId, MoveVector move)
        {
            var game = await _repository.Get(new GameGetFilter(PlayerCode: playerCode));

            if (game == null)
                return new MoveResult(
                    Message: "game not found");

            if(game.State is not GameState.Running)
                return new MoveResult(
                    Message: "game is not started");

            var moveFrom = game.FirstPlayerCode == playerCode
                ? GamePlayer.FirstPlayer
                : GamePlayer.SecondPlayer;

            if (moveFrom != game.AwaitableMove)
                return new MoveResult(
                    Message: $"move from {moveFrom} is not awaitable now");

            var boardState = await _gameHistoryRepository.GetLastBoardState(game.Id);

            if(previousBoardStateId != boardState.Id)
                return new MoveResult(
                    Message: "wrong previousBoardStateId");

            var result = await _moveManager.TryMove(game, boardState, move);

            if (result.Success)
            {
                if (!result.NewBoardState.GetPossibleMoves().Any())
                {
                    game.FinishDateTime = DateTime.UtcNow;
                    game.State = GameState.Finished;
                    game.Winner = moveFrom;
                    game.AwaitableMove = null;

                    result = result with { AwaitableMove = null };
                }

                result.NewBoardState.PreviousBoardStateId = previousBoardStateId;

                await _gameHistoryRepository.SaveBoardState(result.NewBoardState);
                await _repository.Update(game);
            }
            else
            {
                return new MoveResult(
                    Success: false,
                    Message: result.Message);
            }

            return result;
        }

        public async Task<MoveResult> MoveWithBot(Guid playerCode, Guid previousBoardStateId, MoveVector move)
        {
            var moveResult = await Move(playerCode, previousBoardStateId, move);

            if (moveResult is { Success: true, AwaitableMove: GamePlayer.SecondPlayer })
                await _botNotifier.MoveNotify(moveResult.NewBoardState.GameId);

            return moveResult;
        }

    }
}
