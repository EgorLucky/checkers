using System;

namespace DomainLogic.ResultModels
{
    public record GameRegisterSecondPlayerResult(
        bool Success = true,
        string Message = "",
        Guid? Code = null);
}
