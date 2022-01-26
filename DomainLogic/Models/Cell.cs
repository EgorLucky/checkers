namespace DomainLogic.Models
{
    public class Cell
    {
        private CellColor color;

        public Cell(CellColor color)
        {
            this.color = color;
        }

        public Checker Checker { get; internal set; }
    }
}