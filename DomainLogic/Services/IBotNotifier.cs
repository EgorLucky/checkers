using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public interface IBotNotifier
    {
        public Task RegisterNotify(Guid gameId);
    }
}
