using System.Collections.Generic;

namespace DomainLogic.Models
{
    public record Move(
        MoveVector MoveVector,
        CellCoordinate CapturableCheckerCoordinate = null
    );
}