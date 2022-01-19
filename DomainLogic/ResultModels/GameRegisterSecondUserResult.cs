using System;

namespace DomainLogic.ResultModels
{
    public record GameRegisterSecondUserResult(
        bool Success = true,
        string Message = "",
        Guid? Code = null);
}
