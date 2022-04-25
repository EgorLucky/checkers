using System.Collections.Generic;

namespace DomainLogic.Models
{
    public record Move(
        CellCoordinate From,
        CellCoordinate To,
        CellCoordinate CapturableCheckerCoordinate = null
    );
}