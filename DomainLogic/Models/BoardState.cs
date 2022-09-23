using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Models
{
    public record BoardState
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Board Board { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// Previous boardStateId or game id in case of first state
        /// </summary>
        public Guid PreviousBoardStateId { get; set; }

        public IEnumerable<Move> GetPossibleMoves() => Board
                                                        .Cells
                                                        .Where(c => c is { Checker: not null, Checker.PossibleMoves: not null })
                                                        .SelectMany(c => c.Checker.PossibleMoves);

        internal void AddPossibleMoves(List<Move> possibleMoves)
        {
            foreach(var possibleMove in possibleMoves
                                            .GroupBy(p => p.MoveVector.From)
                                            .Select(p => new 
                                            {
                                                From = p.Key, 
                                                Moves = p.ToList()
                                            }))
            {
                var checker = Board
                                .Cells
                                .Where(c => c.Coordinate == possibleMove.From)
                                .Select(c => c.Checker)
                                .First();
                checker.PossibleMoves.AddRange(possibleMove.Moves);
            }
        }
    }
}
