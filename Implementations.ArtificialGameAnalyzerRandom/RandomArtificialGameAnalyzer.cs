using DomainLogic.Models;
using DomainLogic.Services;
using System.Security.Cryptography;

namespace Implementations.ArtificialAnalyzerRandom
{
    public class RandomArtificialGameAnalyzer : IArtificialGameAnalyzer
    {
        public async Task<MoveVector> CreateMove(List<Cell> myCheckersCells, List<Cell> opponentCheckersCells, List<Move> possibleMoves)
        {
            var index = RandomNumberGenerator.GetInt32(0, possibleMoves.Count);

            return possibleMoves[index].MoveVector;
        }
    }
}