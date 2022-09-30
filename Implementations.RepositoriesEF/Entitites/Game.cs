using DomainLogic.Models;

namespace Implementations.RepositoriesEF.Entitites
{
    public class Game
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? FinishDateTime { get; set; }
        public GameState? State { get; set; }
        public GamePlayer? AwaitableMove { get; set; }
        public string CheckerCellColor { get; set; }
        public string FirstPlayerCheckerColor { get; set; }
        public string OpponentCheckerColor { get; set; }
        public string NonPlayableCellColor { get; set; }
        public BoardSide FirstPlayerBoardSide { get; set; }
        public BoardSide SecondPlayerBoardSide { get; set; }
        public GamePlayer? Winner { get; set; }
        public List<Player> Players { get; set; }
    }
}