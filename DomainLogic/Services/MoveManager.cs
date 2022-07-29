using DomainLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class MoveManager
    {
        public async Task<BoardState> InitializeHistory(Game game)
        {
            var boardState = CreateBoardState(game.Id);

            var board = new Board(game.CheckerCellColor, game.NonPlayableCellColor);
            board.Fill(game.FirstPlayerCheckerColor, game.OpponentCheckerColor);

            boardState.Board = board;
            boardState.PossibleMoves = board.FindPossibleMoves(game);

            return boardState;
        }

        BoardState CreateBoardState(Guid gameId) => new BoardState
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            CreateDateTime = DateTimeOffset.Now
        };

        public async Task<MoveResult> TryMove(Game game, BoardState boardState, MoveVector moveVector)
        {
            var move = default(Move);
            if ((move = boardState.PossibleMoves.FirstOrDefault(m => m.MoveVector == moveVector)) == null)
                return new MoveResult(
                    Success: false,
                    Message: "wrong move");

            //clone BoardState
            var newBoardState = CreateBoardState(boardState.GameId);
            var board = boardState.Board.Clone();
            newBoardState.Board = board;

            var cellFrom = board[moveVector.From];
            var cellTo = board[moveVector.To];

            var movingChecker = cellFrom.Checker;
            cellFrom.Checker = null;
            cellTo.Checker = movingChecker;

            if(move.CapturableCheckerCoordinate != null)
            {
                var cellWithCapturableCell = board[move.CapturableCheckerCoordinate];
                cellWithCapturableCell.Checker = null;
            }
            //find possible moves for player who just moved
            boardState.PossibleMoves = board.FindPossibleMoves(cellTo);

            if (boardState.PossibleMoves.Any() == false)
            {
                //find possible moves for opponent
                SwitchAwaitableMove(game);
                boardState.PossibleMoves = board.FindPossibleMoves(game); 
            }
            //return data
            return new MoveResult(
                Success: true,
                NewBoardState: newBoardState,
                AwaitableMove: game.AwaitableMove);
        }

        private void SwitchAwaitableMove(Game game)
        {
            int int32AwaitableMove = (int)game.AwaitableMove;
            int32AwaitableMove++;
            int32AwaitableMove = int32AwaitableMove % Enum.GetValues<AwaitableMove>().Length;
            game.AwaitableMove = (AwaitableMove)int32AwaitableMove;
        }
    }
}
