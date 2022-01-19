using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Filters
{
    public record GameGetFilter(
        Guid? Id = null,
        Guid? FirstPlayerCode = null);
}
