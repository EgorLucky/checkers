using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.ParameterModels
{
    public record GameCreateDTO(
        string MyCeckerColor = "",
        string OpponentCheckerColor = "",
        string CheckerCellColor = "",
        string NonPlayableCellColor = "");
}
