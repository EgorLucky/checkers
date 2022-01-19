using DomainLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.ResultModels
{
    public record GameStartResult(
        bool Success = true,
        string Message = "",
        AwaitableMove? AwaitableMove = null);
}
