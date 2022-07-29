using DomainLogic.Models;
using System;
using System.Collections.Generic;

namespace DomainLogic
{
    public class MenCheckerMoveSearcher : ICheckerMoveSearcher
    {
        public List<Move> SearchCaptureMoves(CellCoordinate coordinate, Board board)
        {
            var cell = board[coordinate];
            var checker = cell.Checker;
            var result = new List<Move>();

            var searchCaptureDelegate = new Action<bool, bool, bool>(
                (coordinateConditionResult, horizontalSign, verticalSign)
                    => SearchCapture(
                        board,
                        coordinate,
                        result,
                        coordinateConditionResult,
                        horizontalSign,
                        verticalSign));

            searchCaptureDelegate(
                coordinate.Horizontal > BoardHorizontalCoordinates.B 
                && coordinate.Vertical > BoardVerticalCoordinates.Two,
                false,
                false);

            searchCaptureDelegate(
                coordinate.Horizontal > BoardHorizontalCoordinates.B
                && coordinate.Vertical <= BoardVerticalCoordinates.Six,
                false,
                true);

            searchCaptureDelegate(
                coordinate.Horizontal <= BoardHorizontalCoordinates.F
                && coordinate.Vertical <= BoardVerticalCoordinates.Six,
                true,
                true);

            searchCaptureDelegate(
                coordinate.Horizontal <= BoardHorizontalCoordinates.F
                && coordinate.Vertical > BoardVerticalCoordinates.Two,
                true,
                false);

            return result;
        }

        public List<Move> SearchSimpleMoves(CellCoordinate coordinate, Board board, string firstPlayerCheckerColor)
        {
            var result = new List<Move>();
            var cell = board[coordinate];
            var checker = cell.Checker;

            var searchSimpleDelegate = new Action<BoardHorizontalCoordinates, BoardVerticalCoordinates, bool, bool>(
                (horizontal, vertical, horizontalSign, verticalSign) 
                    => SearchSimple(
                        board, 
                        coordinate, 
                        result,
                        horizontal,
                        vertical,
                        horizontalSign,
                        verticalSign));

            //TODO: make property to know checker vector
            if (checker.Color == firstPlayerCheckerColor)
            {
                //then search moves up from start of board
                searchSimpleDelegate(
                    BoardHorizontalCoordinates.A, 
                    BoardVerticalCoordinates.Eight, 
                    false, 
                    true);

                searchSimpleDelegate(
                    BoardHorizontalCoordinates.H,
                    BoardVerticalCoordinates.Eight,
                    true,
                    true);
            }
            else
            {
                //then search moves down from end of board
                searchSimpleDelegate(
                    BoardHorizontalCoordinates.A,
                    BoardVerticalCoordinates.One,
                    false,
                    false);

                searchSimpleDelegate(
                    BoardHorizontalCoordinates.H,
                    BoardVerticalCoordinates.One,
                    true,
                    false);
            }

            return result;
        }

        void SearchSimple(
            Board board,
            CellCoordinate coordinate,
            List<Move> result,
            BoardHorizontalCoordinates horizontal, 
            BoardVerticalCoordinates vertical,
            bool horizontalSign,
            bool verticalSign
            )
        {
            var diffHorizontal = horizontalSign ? 1 : -1;
            var diffVertical = verticalSign ? 1 : -1;

            var moveToCoordinate = new CellCoordinate(
                coordinate.Horizontal + diffHorizontal, 
                coordinate.Vertical + diffVertical);

            if (coordinate.Horizontal != horizontal
                    && coordinate.Vertical != vertical
                    && board[moveToCoordinate].Checker == null)
            {
                result.Add(new Move((coordinate, moveToCoordinate)));
            }
        }

        void SearchCapture(
            Board board,
            CellCoordinate coordinate,
            List<Move> result,
            bool coordinateConditionResult,
            bool horizontalSign,
            bool verticalSign
            )
        {
            var cell = board[coordinate];
            var checker = cell.Checker;

            var multiplierHorizontal = horizontalSign ? 1 : -1;
            var multiplierVertical = verticalSign ? 1 : -1;

            var moveToCoordinate = (
                coordinate.Horizontal + 2 * multiplierHorizontal,
                coordinate.Vertical + 2 * multiplierVertical);

            var capturableCheckerCoordinate = (
                coordinate.Horizontal + multiplierHorizontal,
                coordinate.Vertical + multiplierVertical);

            if (coordinateConditionResult
                && board[capturableCheckerCoordinate].Checker != null
                && board[capturableCheckerCoordinate].Checker.Color != checker.Color
                && board[moveToCoordinate].Checker == null)
            {
                result.Add(new Move((coordinate, moveToCoordinate), capturableCheckerCoordinate));
            }
        }
    }
}
