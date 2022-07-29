using DomainLogic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface ICheckerMoveSearcher
    {
        List<Move> SearchCaptureMoves(CellCoordinate coordinate, Board board);

        List<Move> SearchSimpleMoves(CellCoordinate coordinate, Board board, string firstPlayerCheckerColor);
    }
}
