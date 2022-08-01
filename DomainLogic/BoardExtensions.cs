using DomainLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    internal static class BoardExtensions
    {
        const int playerCheckersCount = 12;
        public static void Fill(this Board board, string firstPlayerCheckerColor, string secondPlayerCheckerColor)
        {
            var isFirstCellWithChecker = true;
            var currentSideCheckersNotPlacedCount = playerCheckersCount;
            var checkerColor = firstPlayerCheckerColor;
            var isCellWithChecker = isFirstCellWithChecker;

            foreach (var vertical in Enum.GetValues<BoardVerticalCoordinates>())
            {
                foreach (var horizontal in Enum.GetValues<BoardHorizontalCoordinates>())
                {
                    var currentCellColor = isCellWithChecker ? 
                        board.CheckerCellColor : 
                        board.NonPlayableCellColor;

                    var cell = new Cell(currentCellColor);

                    if (currentCellColor == board.CheckerCellColor
                        && currentSideCheckersNotPlacedCount > 0
                        && vertical != BoardVerticalCoordinates.Four
                        && vertical != BoardVerticalCoordinates.Five)
                    {
                        cell.Checker = new Checker(checkerColor);

                        currentSideCheckersNotPlacedCount--;

                        if (currentSideCheckersNotPlacedCount == 0)
                        {
                            currentSideCheckersNotPlacedCount = playerCheckersCount;
                            checkerColor = secondPlayerCheckerColor;
                        }
                    }

                    board[horizontal, vertical] = cell;

                    if (horizontal != BoardHorizontalCoordinates.H)
                        isCellWithChecker = !isCellWithChecker;
                }
            }
        }

        public static Board Clone(this Board board)
        {
            return new Board(board.CheckerCellColor, board.NonPlayableCellColor)
            {
                Cells = board.Cells.Select(c => new Cell(c.Color, c.Coordinate with { })
                {
                    Checker = c.Checker with { }
                })
                .ToList()
            };
        }

        public static List<Move> FindPossibleMoves(this Board board, Game game)
        {
            return FindPossibleMoves(board, game, null);
        }

        public static List<Move> FindPossibleMoves(this Board board, Cell lastMovedCheckersCell)
        {
            return FindPossibleMoves(board, null, lastMovedCheckersCell);
        }
        //its better to use checker color instead of awaitable move 
        static List<Move> FindPossibleMoves(Board board, Game game, Cell lastMovedCheckersCell)
        {
            var checkerColor = game?.AwaitableMove == AwaitableMove.FirstPlayer
                ? game?.FirstPlayerCheckerColor
                : game?.OpponentCheckerColor;

            if(lastMovedCheckersCell != null)
                checkerColor = lastMovedCheckersCell.Checker.Color;

            var cellsWithCheckersEnumerable = board
                                .Cells
                                .Where(c => c.Checker != null)
                                .Where(c => c.Checker.Color == checkerColor);

            if (lastMovedCheckersCell != null)
                cellsWithCheckersEnumerable = cellsWithCheckersEnumerable
                    .OrderBy(c => c.Coordinate != lastMovedCheckersCell.Coordinate);

            var cellsWithCheckers = cellsWithCheckersEnumerable.ToList();

            var result = new List<Move>();
            var captureMovesFound = false;

            foreach (var cellWithChecker in cellsWithCheckers)
            {
                ICheckerMoveSearcher searcher =
                            cellWithChecker.Checker.Role == CheckerRole.Men
                                ? new MenCheckerMoveSearcher()
                                : new KingCheckerMoveSearcher();

                var captureMoves = searcher.SearchCaptureMoves(cellWithChecker.Coordinate, board);

                if (captureMoves.Count > 0)
                {
                    result.AddRange(captureMoves);
                    captureMovesFound = true;
                }
                else if (captureMovesFound == false)
                {
                    var simpleMoves = searcher.SearchSimpleMoves(cellWithChecker.Coordinate, board, game.FirstPlayerCheckerColor);
                    result.AddRange(simpleMoves);
                }

                if (cellWithChecker.Coordinate == lastMovedCheckersCell?.Coordinate)
                    break;
            }

            return result;
        }
    }
}
