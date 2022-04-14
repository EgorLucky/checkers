namespace DomainLogic.Models
{
    public class Cell
    {
        public Cell(CellColor color, CellCoordinate coordinate = null)
        {
            Color = color;
            Coordinate = coordinate;
        }

        public CellColor Color { get; set; }
        public Checker Checker { get; set; }
        public CellCoordinate Coordinate { get; set; }
    }
}