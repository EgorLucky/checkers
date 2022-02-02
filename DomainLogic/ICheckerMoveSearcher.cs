using DomainLogic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface ICheckerMoveSearcher
    {
        List<PossibleMove> SearchCaptureMoves(CellCoordinate coordinate, Board board);

        List<PossibleMove> SearchSimpleMoves(CellCoordinate coordinate, Board board);
    }
}
