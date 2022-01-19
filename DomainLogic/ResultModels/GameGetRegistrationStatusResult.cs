namespace DomainLogic.ResultModels
{
    public record GameGetRegistrationStatusResult(
        bool Success = true,
        string Message = "",
        bool SecondUserRegistred = false);
}
