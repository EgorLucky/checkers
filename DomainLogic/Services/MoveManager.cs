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
            var boardState = new BoardState
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                CreateDateTime = DateTimeOffset.Now
            };

            var checkerCellColor = (new Random()).Next(2) == 1 
                ? CellColor.Black 
                : CellColor.White;
            var board = new Board(checkerCellColor);

            boardState.Board = board;
            boardState.PossibleMoves = FindPossibleMoves(board, game.AwaitableMove.Value);
            //save


            return boardState;
        }

        private List<PossibleMove> FindPossibleMoves(Board board, AwaitableMove awaitableMove)
        {
            var checkerColor = awaitableMove == AwaitableMove.FirstPlayer 
                ? CheckerColor.White 
                : CheckerColor.Black;

            var cellsWithCheckers = board
                                .Cells
                                .Where(c => c.Value.Checker.Color == checkerColor)
                                .ToList();

            var result = new List<PossibleMove>();

            foreach(var cellWithChecker in cellsWithCheckers)
            {
                //search capture moves
                //if found add to result and continue
                //else found other moves
                //add founded to result
            }

            return result;
        }
    }
}
