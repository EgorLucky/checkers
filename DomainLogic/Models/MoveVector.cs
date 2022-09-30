namespace DomainLogic.Models
{
    public record MoveVector(
        CellCoordinate From,
        CellCoordinate To)
    {
        public static implicit operator MoveVector(
            (CellCoordinate, CellCoordinate) moveVectorTuple)
                    => new MoveVector(moveVectorTuple.Item1, moveVectorTuple.Item2);
    };
}