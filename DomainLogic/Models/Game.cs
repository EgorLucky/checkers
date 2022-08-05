using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Models
{
    public record Game
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
        public DateTimeOffset? StartDateTime { get; set; }
        public DateTimeOffset? FinishDateTime { get; set; }
        public GameState? State { get; set; }
        public GamePlayer? AwaitableMove { get; set; }
        public Guid? FirstPlayerCode { get; internal set; }
        public Guid? SecondPlayerCode { get; internal set; }
        public string CheckerCellColor { get; internal set; }
        public string FirstPlayerCheckerColor { get; internal set; }
        public string OpponentCheckerColor { get; set; }
        public string NonPlayableCellColor { get; set; }
        public BoardSide FirstPlayerBoardSide { get; set; }
        public BoardSide SecondPlayerBoardSide { get; set; }
        public GamePlayer? Winner { get;  set; }
    }
}
