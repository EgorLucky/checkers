using System.Collections.Generic;

namespace DomainLogic.Models
{
    public record PossibleMove(
        CellCoordinate From,
        CellCoordinate To,
        CellCoordinate CapturableCheckerCoordinate = null
    );
}