using DomainLogic.Models;
using System;
using System.Collections.Generic;

namespace DomainLogic
{
    public class KingCheckerMoveSearcher : ICheckerMoveSearcher
    {
        public List<PossibleMove> SearchCaptureMoves(CellCoordinate coordinate, Board board)
        {
            var result = new List<PossibleMove>();

            var searchCaptureDelegate = new Action<
                bool,
                bool,
                bool,
                Func<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool>,
                Func<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool>,
                BoardHorizontalCoordinates,
                BoardVerticalCoordinates>(
                (cellCoordinateConditionResult,
                horizontalSign,
                verticalSign,
                firstCycleCondition,
                secondCycleCondition,
                restrictedHorizontalValue,
                restrictedVerticalValue) => 
                SearchCapture(
                    board,
                    coordinate,
                    result,
                    cellCoordinateConditionResult,
                    horizontalSign,
                    verticalSign,
                    firstCycleCondition,
                    secondCycleCondition,
                    restrictedHorizontalValue,
                    restrictedVerticalValue));

            (var x, var y) = coordinate;

            //search left down
            searchCaptureDelegate(
                x > BoardHorizontalCoordinates.B && y > BoardVerticalCoordinates.Two,
                false,
                false,
                (h, v) => h > BoardHorizontalCoordinates.A && v > BoardVerticalCoordinates.One,
                (h, v) => h >= BoardHorizontalCoordinates.A && v >= BoardVerticalCoordinates.One,
                BoardHorizontalCoordinates.A,
                BoardVerticalCoordinates.One);

            //search left up
            searchCaptureDelegate(
                x > BoardHorizontalCoordinates.B && y <= BoardVerticalCoordinates.Six,
                false,
                true,
                (h, v) => h > BoardHorizontalCoordinates.A && v < BoardVerticalCoordinates.Eight,
                (h, v) => h > BoardHorizontalCoordinates.A && v <= BoardVerticalCoordinates.Eight,
                BoardHorizontalCoordinates.A,
                BoardVerticalCoordinates.Eight);

            //search right up
            searchCaptureDelegate(
                x <= BoardHorizontalCoordinates.F && y <= BoardVerticalCoordinates.Six,
                true,
                true,
                (h, v) => h < BoardHorizontalCoordinates.H && v < BoardVerticalCoordinates.Eight,
                (h, v) => h <= BoardHorizontalCoordinates.H && v <= BoardVerticalCoordinates.Eight,
                BoardHorizontalCoordinates.H,
                BoardVerticalCoordinates.Eight);

            //search right down
            searchCaptureDelegate(
                x <= BoardHorizontalCoordinates.F && y > BoardVerticalCoordinates.Two,
                true,
                false,
                (h, v) => h < BoardHorizontalCoordinates.H && v > BoardVerticalCoordinates.One,
                (h, v) => h <= BoardHorizontalCoordinates.H && v >= BoardVerticalCoordinates.One,
                BoardHorizontalCoordinates.H,
                BoardVerticalCoordinates.One);

            return result;
        }

        void SearchCapture(
            Board board,
            CellCoordinate coordinate,
            List<PossibleMove> result,
            bool cellCoordinateConditionResult,
            bool horizontalSign,
            bool verticalSign,
            Func<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool> firstCycleCondition,
            Func<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool> secondCycleCondition,
            BoardHorizontalCoordinates restrictedHorizontalValue,
            BoardVerticalCoordinates restrictedVerticalValue)
        {
            if (cellCoordinateConditionResult == false)
                return;

            var checker = board[coordinate].Checker;
            (var x, var y) = coordinate;

            BoardHorizontalCoordinates i;
            BoardVerticalCoordinates j;

            Func<BoardHorizontalCoordinates, BoardHorizontalCoordinates> changeHorizontalCounter = horizontalSign
                ? (h) => h + 1
                : (h) => h - 1;
            Func<BoardVerticalCoordinates, BoardVerticalCoordinates> changeVerticalCounter = verticalSign
                ? (v) => v + 1
                : (v) => v - 1;

            for (i = changeHorizontalCounter(x), j = changeVerticalCounter(y);
                firstCycleCondition(i, j);
                i = changeHorizontalCounter(i), changeVerticalCounter(j))
            {
                if (board[i, j].Checker != null)
                {
                    var currentCellChecker = board[i, j].Checker;
                    var currentCoordinate = (i, j);
                    if (currentCellChecker.Color != checker.Color
                        && i != restrictedHorizontalValue
                        && j != restrictedVerticalValue
                        && board[(i = changeHorizontalCounter(i)), j = changeVerticalCounter(j)].Checker == null)
                    {
                        result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                        for (i = changeHorizontalCounter(i), j = changeVerticalCounter(j);
                            secondCycleCondition(i, j)
                            && board[i, j].Checker == null;
                            i = changeHorizontalCounter(i), j = changeVerticalCounter(j))
                            result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                    }
                    break;
                }
            }
        }


        public List<PossibleMove> SearchSimpleMoves(CellCoordinate coordinate, Board board)
        {
            var result = new List<PossibleMove>();

            var checker = board[coordinate].Checker;
            (var x, var y) = coordinate;
            BoardHorizontalCoordinates i;
            BoardVerticalCoordinates j;

            //left up
            for(i = x - 1, j = y + 1; 
                i >= BoardHorizontalCoordinates.A 
                && j <= BoardVerticalCoordinates.Eight; 
                i--, j++)
            {
                if (board[i, j].Checker != null)
                    break;
                else
                    result.Add(new PossibleMove(coordinate, (i, j)));
            }

            //right up
            for (i = x + 1, j = y + 1;
                i <= BoardHorizontalCoordinates.H
                && j <= BoardVerticalCoordinates.Eight;
                i++, j++)
            {
                if (board[i, j].Checker != null)
                    break;
                else
                    result.Add(new PossibleMove(coordinate, (i, j)));
            }

            //right down
            for (i = x + 1, j = y - 1;
                i <= BoardHorizontalCoordinates.H
                && j >= BoardVerticalCoordinates.One;
                i++, j--)
            {
                if (board[i, j].Checker != null)
                    break;
                else
                    result.Add(new PossibleMove(coordinate, (i, j)));
            }

            //left down
            for (i = x - 1, j = y - 1;
                i >= BoardHorizontalCoordinates.A
                && j >= BoardVerticalCoordinates.One;
                i--, j--)
            {
                if (board[i, j].Checker != null)
                    break;
                else
                    result.Add(new PossibleMove(coordinate, (i, j)));
            }

            return result;
        }
    }
}
