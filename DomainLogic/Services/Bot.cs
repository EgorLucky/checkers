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
        private readonly IArtificialGameAnalyzer _gameAnalyzer;

        public Bot(
            IGameServiceClient service, 
            IBotRepository botRepository, 
            IArtificialGameAnalyzer gameAnalyzer)
        {
            _service = service;
            _botRepository = botRepository;
            _gameAnalyzer = gameAnalyzer;
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

        public async Task Move(Guid gameId)
        {
            var playerGameData = await  _botRepository.Get(gameId);
            var gameInfo = await _service.GetInfo(gameId);

            var boardState = gameInfo.BoardState;
            var possibleMoves = boardState.GetPossibleMoves().ToList();
            var board = boardState.Board;
            var myColor = board
                            .Cells
                            .Where(c => c.Coordinate == possibleMoves.First().MoveVector.From)
                            .Select(c => c.Checker.Color)
                            .First();

            do
            {
                var myCheckersCells = board
                                        .Cells
                                        .Where(c => c.Checker != null
                                                    && c.Checker.Color == myColor)
                                        .ToList();

                var opponentCheckersCells = board
                                        .Cells
                                        .Where(c => c.Checker != null
                                                    && c.Checker.Color != myColor)
                                        .ToList();

                var move = await _gameAnalyzer.CreateMove(myCheckersCells, opponentCheckersCells, possibleMoves);

                var moveResult = await _service.MakeMove(move, playerGameData.PlayerCode);

                if (moveResult.Success == false)
                {
                    //do something
                    break;
                }

                if (moveResult.AwaitableMove != Models.GamePlayer.SecondPlayer)
                    break;

                boardState = moveResult.NewBoardState;
                board = boardState.Board;
                possibleMoves = boardState.GetPossibleMoves().ToList();

            } while (true);
        }
    }
}
