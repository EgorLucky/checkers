namespace DomainLogic.Models
{
    public class Cell
    {
        public Cell(string color, CellCoordinate coordinate = null)
        {
            Color = color;
            Coordinate = coordinate;
        }

        public string Color { get; set; }
        public Checker Checker { get; set; }
        public CellCoordinate Coordinate { get; set; }
    }
}