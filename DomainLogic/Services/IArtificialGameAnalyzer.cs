using DomainLogic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public interface IArtificialGameAnalyzer
    {
        Task<MoveVector> CreateMove(List<Cell> myCheckersCells, List<Cell> opponentCheckersCells, List<Move> possibleMoves);
    }
}