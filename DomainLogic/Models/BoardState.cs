using System;
using System.Collections.Generic;

namespace DomainLogic.Models
{
    public record BoardState
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Board Board { get; set; }
        public List<Move> PossibleMoves { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
    }
}
