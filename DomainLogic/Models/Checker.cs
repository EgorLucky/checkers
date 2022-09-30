using System.Collections.Generic;

namespace DomainLogic.Models
{
    public enum CheckerRole { Men, King }
    public record Checker(string Color, BoardSide BoardSide, CheckerRole Role = CheckerRole.Men, List<Move> PossibleMoves = null);
}