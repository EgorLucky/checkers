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

    public enum CellColor { Black, White }
    public enum CheckerColor { Black, White }


    public class Board
    {
        const int playerCheckersCount = 12;
        public Board(CellColor checkerCellColor)
        {
            var isBlack = true;
            var currentSideCheckersNotPlacedCount = playerCheckersCount;
            var checkerColor = CheckerColor.White;

            foreach(var horizontal in Enum.GetValues<BoardHorizontalCoordinates>())
            {
                foreach (var vertical in Enum.GetValues<BoardVerticalCoordinates>())
                {
                    var color = isBlack ? CellColor.Black : CellColor.White;

                    var cell = new Cell(color);

                    if (color == checkerCellColor 
                        && currentSideCheckersNotPlacedCount > 0
                        && vertical != BoardVerticalCoordinates.Four 
                        && vertical != BoardVerticalCoordinates.Five)
                    {
                        cell.Checker = new Checker(checkerColor);

                        currentSideCheckersNotPlacedCount--;

                        if(currentSideCheckersNotPlacedCount == 0)
                        {
                            currentSideCheckersNotPlacedCount = playerCheckersCount;
                            checkerColor = CheckerColor.Black;
                        }
                    }

                    this[horizontal, vertical] = cell;

                    if (vertical != BoardVerticalCoordinates.Eight)
                        isBlack = !isBlack;
                }
            }

        }


        public Cell this[BoardHorizontalCoordinates horizontal, BoardVerticalCoordinates vertical] 
        {
            get => Cells[new CellCordinate(horizontal, vertical)];

            set => Cells[new CellCordinate(horizontal, vertical)] = value;
        }

        public Dictionary<CellCordinate, Cell> Cells { get; set; } = new Dictionary<CellCordinate, Cell>();
    }

    public record CellCordinate(
        BoardHorizontalCoordinates Horizontal, 
        BoardVerticalCoordinates Vertical);
}
