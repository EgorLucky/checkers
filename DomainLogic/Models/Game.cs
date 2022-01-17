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

    }
}
