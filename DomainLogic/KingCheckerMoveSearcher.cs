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

            var searchSimpleDelegate = new Action<
                bool,
                bool,
                Func<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool>>(
                (horizontalSign, 
                verticalSign,
                cycleCondition) => 
                SearchSimple(
                    board, 
                    coordinate, 
                    result, 
                    horizontalSign, 
                    verticalSign, 
                    cycleCondition)); 

            var checker = board[coordinate].Checker;
            (var x, var y) = coordinate;
            BoardHorizontalCoordinates i;
            BoardVerticalCoordinates j;

            //left up
            searchSimpleDelegate(
                false, 
                true, 
                (h, v) => h >= BoardHorizontalCoordinates.A && v <= BoardVerticalCoordinates.Eight);

            //right up
            searchSimpleDelegate(
                true,
                true,
                (h, v) => h <= BoardHorizontalCoordinates.H && v <= BoardVerticalCoordinates.Eight);

            //right down
            searchSimpleDelegate(
                true,
                false,
                (h, v) => h <= BoardHorizontalCoordinates.H && v >= BoardVerticalCoordinates.One);

            //left down
            searchSimpleDelegate(
                false,
                false,
                (h, v) => h >= BoardHorizontalCoordinates.A && v >= BoardVerticalCoordinates.One);

            return result;
        }

        void SearchSimple(
            Board board,
            CellCoordinate coordinate,
            List<PossibleMove> result,
            bool horizontalSign,
            bool verticalSign,
            Func<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool> cycleCondition
            )
        {
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
                cycleCondition(i, j);
                changeHorizontalCounter(i), changeVerticalCounter(j))
            {
                if (board[i, j].Checker != null)
                    break;
                else
                    result.Add(new PossibleMove(coordinate, (i, j)));
            }
        }
    }
}
