using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Models
{
    public enum BoardHorizontalCoordinates
    {
        A, B, C, D, E, F, G, H
    }

    public enum BoardVerticalCoordinates
    {
        One, Two, Three, Four, Five, Six, Seven, Eight
    }

    public class Board
    {
        public Board(string checkerCellColor, string nonPlayableCellColor)
        {
            CheckerCellColor = checkerCellColor;
            NonPlayableCellColor = nonPlayableCellColor;
        }

        public string CheckerCellColor { get; }
        public string NonPlayableCellColor { get; set; }

        public Cell this[BoardHorizontalCoordinates horizontal, BoardVerticalCoordinates vertical] 
        {
            get => Cells.Find(c => c.Coordinate == (horizontal, vertical));

            set
            {
                Cells.RemoveAll(c => c.Coordinate == (horizontal, vertical));
                value.Coordinate = (horizontal, vertical);
                Cells.Add(value);
            }
        }

        public Cell this[CellCoordinate coordinate]
        {
            get => Cells.Find(c => c.Coordinate == coordinate);

            set
            {
                Cells.RemoveAll(c => c.Coordinate == coordinate);
                value.Coordinate = coordinate;
                Cells.Add(value);
            }
        }

        public List<Cell> Cells { get; set; } = new List<Cell>();
    }

    public record CellCoordinate(
        BoardHorizontalCoordinates Horizontal,
        BoardVerticalCoordinates Vertical)
    {
        public static implicit operator CellCoordinate(
            (BoardHorizontalCoordinates, BoardVerticalCoordinates) coordinateTuple) 
                    => new CellCoordinate(coordinateTuple.Item1, coordinateTuple.Item2);

        public static implicit operator (BoardHorizontalCoordinates, BoardVerticalCoordinates)
                                        (CellCoordinate coordinate) =>
                                        (coordinate.Horizontal, coordinate.Vertical);
    };
}
