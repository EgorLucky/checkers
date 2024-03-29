﻿using DomainLogic.Models;
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
            boardState.PreviousBoardStateId = game.Id;

            var board = new Board(game.CheckerCellColor, game.NonPlayableCellColor);
            board.Fill(game.FirstPlayerCheckerColor, game.OpponentCheckerColor, game.FirstPlayerBoardSide, game.SecondPlayerBoardSide);

            boardState.Board = board;
            boardState.AddPossibleMoves(board.FindPossibleMoves(game));

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
            if ((move = boardState.GetPossibleMoves().FirstOrDefault(m => m.MoveVector == moveVector)) == null)
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
            
            var boardVerticalCoordinates = Enum.GetValues<BoardVerticalCoordinates>();
            if(movingChecker.BoardSide == BoardSide.FirstSide && cellTo.Coordinate.Vertical == boardVerticalCoordinates.Max() ||
                movingChecker.BoardSide == BoardSide.SecondSide && cellTo.Coordinate.Vertical == boardVerticalCoordinates.Min())
            {
                movingChecker = movingChecker with { Role = CheckerRole.King };
            }

            cellTo.Checker = movingChecker;

            //if move with capture
            if (move.CapturableCheckerCoordinate != null)
            {
                //remove captured checker
                var cellWithCapturableCell = board[move.CapturableCheckerCoordinate];
                cellWithCapturableCell.Checker = null;

                var possibleMoves = board.FindPossibleMoves(game, cellTo);
                //if there are any capture moves for checker which just did capture
                if(possibleMoves.Any(p => p.CapturableCheckerCoordinate is not null))
                {
                    newBoardState.AddPossibleMoves(possibleMoves);

                    return new MoveResult(
                        Success: true,
                        NewBoardState: newBoardState,
                        AwaitableMove: game.AwaitableMove);
                }
            }
            
            //find possible moves for opponent
            SwitchAwaitableMove(game);
            newBoardState.AddPossibleMoves(board.FindPossibleMoves(game));

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
            int32AwaitableMove = int32AwaitableMove % Enum.GetValues<GamePlayer>().Length;
            game.AwaitableMove = (GamePlayer)int32AwaitableMove;
        }
    }
}
