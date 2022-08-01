using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.ParameterModels
{
    public record GameCreateDTO(
        string MyCeckerColor = "#000000",
        string OpponentCheckerColor = "#FFFFFF",
        string CheckerCellColor = "#000000",
        string NonPlayableCellColor = "#FFFFFF");
}
