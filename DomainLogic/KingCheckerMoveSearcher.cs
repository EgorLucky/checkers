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

            var checker = board[coordinate].Checker;
            (var x, var y) = coordinate;
            //search left down
            if(x > BoardHorizontalCoordinates.B && y > BoardVerticalCoordinates.Two)
            {
                BoardHorizontalCoordinates i;
                BoardVerticalCoordinates j;
                for(i = x - 1, j = y - 1; 
                    i > BoardHorizontalCoordinates.A 
                    && j > BoardVerticalCoordinates.One; 
                    i--, j--)
                {
                    if(board[i, j].Checker != null)
                    {
                        var currentCellChecker = board[i, j].Checker;
                        var currentCoordinate = (i, j);
                        if(currentCellChecker.Color != checker.Color
                            && i != BoardHorizontalCoordinates.A
                            && j != BoardVerticalCoordinates.One
                            //I DONT REMEMBER HOW THIS IS WORKS
                            && board[--i, --j].Checker == null)
                        {
                            result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                            for (i = i - 1, j = j - 1; 
                                i >= BoardHorizontalCoordinates.A 
                                && j >= BoardVerticalCoordinates.One 
                                && board[i, j].Checker == null; 
                                i--, j--)
                                result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                        }
                        break;
                    }

                }
            }
            //search left up
            if (x > BoardHorizontalCoordinates.B && y <= BoardVerticalCoordinates.Six)
            {
                BoardHorizontalCoordinates i;
                BoardVerticalCoordinates j;
                for (i = x - 1, j = y + 1; 
                    i > BoardHorizontalCoordinates.A 
                    && j < BoardVerticalCoordinates.Eight; 
                    i--, j++)
                {
                    if (board[i, j].Checker != null)
                    {
                        var currentCellChecker = board[i, j].Checker;
                        var currentCoordinate = (i, j);
                        if (currentCellChecker.Color != checker.Color
                            && i != BoardHorizontalCoordinates.A
                            && j != BoardVerticalCoordinates.Eight
                            //I DONT REMEMBER HOW THIS IS WORKS
                            && board[--i, ++j].Checker == null)
                        {
                            result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                            for (i = i - 1, j = j + 1; 
                                i > BoardHorizontalCoordinates.A 
                                && j <= BoardVerticalCoordinates.Eight 
                                && board[i, j].Checker == null; 
                                i--, j++)
                                result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                        }
                        break;
                    }
                }
            }
            //search right up
            if (x <= BoardHorizontalCoordinates.F && y <= BoardVerticalCoordinates.Six)
            {
                BoardHorizontalCoordinates i;
                BoardVerticalCoordinates j;
                for (i = x + 1, j = y + 1;
                    i < BoardHorizontalCoordinates.H
                    && j < BoardVerticalCoordinates.Eight;
                    i++, j++)
                {
                    if (board[i, j].Checker != null)
                    {
                        var currentCellChecker = board[i, j].Checker;
                        var currentCoordinate = (i, j);
                        if (currentCellChecker.Color != checker.Color
                            && i != BoardHorizontalCoordinates.H
                            && j != BoardVerticalCoordinates.Eight
                            //I DONT REMEMBER HOW THIS IS WORKS
                            && board[++i, ++j].Checker == null)
                        {
                            result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                            for (i = i + 1, j = j + 1;
                                i <= BoardHorizontalCoordinates.H
                                && j <= BoardVerticalCoordinates.Eight
                                && board[i, j].Checker == null; 
                                i++, j++)
                                result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                        }
                        break;
                    }

                }
            }
            //search right down
            if (x <= BoardHorizontalCoordinates.F && y > BoardVerticalCoordinates.Two)
            {
                BoardHorizontalCoordinates i;
                BoardVerticalCoordinates j;
                for (i = x + 1, j = y - 1;
                    i < BoardHorizontalCoordinates.H
                    && j > BoardVerticalCoordinates.One;
                    i++, j--)
                {
                    if (board[i, j].Checker != null)
                    {
                        var currentCellChecker = board[i, j].Checker;
                        var currentCoordinate = (i, j);
                        if (currentCellChecker.Color != checker.Color
                            && i != BoardHorizontalCoordinates.H
                            && j != BoardVerticalCoordinates.One
                            //I DONT REMEMBER HOW THIS IS WORKS
                            && board[++i, --j].Checker == null)
                        {
                            result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                            for (i = i + 1, j = j - 1;
                                i <= BoardHorizontalCoordinates.H
                                && j >= BoardVerticalCoordinates.One
                                && board[i, j].Checker == null; 
                                i++, j--)
                                result.Add(new PossibleMove(coordinate, (i, j), currentCoordinate));
                        }
                        break;
                    }

                }
            }

            return result;
        }

        //void SearchCapture(
        //    Board board,
        //    CellCoordinate coordinate,
        //    List<PossibleMove> result,
        //    bool cellCoordinateConditionResult,
        //    )


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
